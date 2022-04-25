using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;

namespace Com.BowenIvanov.BoatCombat
{
    [RequireComponent(typeof(MeshRenderer))]
    public class CapturePoint : Photon.MonoBehaviour, IPunObservable
    {
        [Tooltip("0: Neutral")]
        [SerializeField] int teamControlling = 0;
        [SerializeField] int maxHealth = 100;
        [SerializeField] float updateHealthPerSecond;
        [SerializeField] List<PlayerManager> contesting = new List<PlayerManager>();
        [SerializeField] PlayerManager playerControlling;
        [SerializeField] float distanceToStartFade = 5;

        int health = 0;

        int teamContesting = 0;

        float timer = 0;

        MeshRenderer mr;
        Material mat;
        Image img;

        [SerializeField] Slider healthBar;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (stream.isWriting)
                {
                    //Send Master Client health
                    stream.SendNext(health);
                    stream.SendNext(teamControlling);
                }
            }
            else
            {
                if (info.sender.IsMasterClient)
                {
                    if (!stream.isWriting)
                    {
                        //recive data from master client
                        this.health = (int)stream.ReceiveNext();
                        this.teamControlling = (int)stream.ReceiveNext();
                    }
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //updateHealthPerSecond;
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;

            mr = GetComponent<MeshRenderer>();
            img = GetComponentInChildren<Image>();
            mat = mr.material;
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.isMasterClient)
            {
                if (timer < 0)
                {
                    updateHealth();
                    timer = updateHealthPerSecond;
                }
                timer -= Time.deltaTime;
            }

            updateUI();

            
        }

        public int getWinning()
        {
            return teamControlling;
        }

        void updateHealth()
        {
            teamContesting = 0;
            foreach(PlayerManager pm in contesting)
            {
                teamContesting += pm.getTeam();
            }
            //Debug.Log(string.Format("Team Contesting: {0}; Team Controlling: {1}; Health: {2}", teamContesting, teamControlling, health));

            if (teamContesting == 0)
            {
                return;
            }
            else if (teamControlling == teamContesting)
            {
                health++;
                if(health >= maxHealth)
                {
                    health = maxHealth;
                }
            }
            else if (teamControlling != teamContesting)
            {
                if (health <= 0)
                {
                    teamControlling = teamContesting;
                    return;
                }
                health--;
            }
        }

        void updateUI()
        {
            healthBar.value = health;
            Color color = healthBar.gameObject.GetComponent<SliderController>().setColors(teamControlling, (float)health / maxHealth);
            mat.color = new Color(color.r, color.g, color.b, mat.color.a);

            Vector3 plrPosition = PlayerManager.LocalPlayerInstance.transform.position;
            float distance = (transform.position - plrPosition).magnitude;
            float opacity = distance / (distanceToStartFade);
            //img.color = color;
            img.color = new Color(color.r, color.g, color.b, Mathf.Clamp(opacity, 0.0f, 1.0f));

            

        }

        public bool isControllingAtMaxHealth()
        {
            if(health == maxHealth)
            {
                return true;
            }
            return false;
        }

        public int getControlling()
        {
            return teamControlling;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PlayerManager pm))
            {
                contesting.Add(pm);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out PlayerManager pm))
            {
                contesting.Remove(pm);
            }
        }

        

        
    }
}
