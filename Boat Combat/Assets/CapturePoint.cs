using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;

namespace Com.BowenIvanov.BoatCombat
{
    public class CapturePoint : Photon.MonoBehaviour, IPunObservable
    {
        [Tooltip("0: Neutral")]
        [SerializeField] int teamControlling = 0;
        [SerializeField] int maxHealth = 100;
        [SerializeField] float updateHealthPerSecond;
        [SerializeField] List<PlayerManager> contesting = new List<PlayerManager>();


        int health = 0;

        int teamContesting = 0;

        float timer = 0;

        [SerializeField] Slider healthBar;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (info.sender.IsMasterClient)
            {
                if (stream.isWriting)
                {
                    stream.SendNext(teamContesting);
                    stream.SendNext(teamControlling);
                }
                else
                {
                    this.teamContesting = (int)stream.ReceiveNext();
                    this.teamControlling = (int)stream.ReceiveNext();
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            //updateHealthPerSecond;
            healthBar.maxValue = maxHealth;
            healthBar.minValue = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if(timer < 0)
            {
                updateHealth();
                timer = updateHealthPerSecond;
            }

            updateUI();

            timer -= Time.deltaTime;
        }

        void updateHealth()
        {
            teamContesting = 0;
            foreach(PlayerManager pm in contesting)
            {
                teamContesting += pm.getTeam();
            }
            Debug.Log(string.Format("Team Contesting: {0}; Team Controlling: {1}; Health: {2}", teamContesting, teamControlling, health));

            if (teamContesting == 0)
            {
                return;
            }
            else if (teamControlling == teamContesting)
            {

                health++;
                if(health == maxHealth)
                {
                    GameManager.self.WinState(teamControlling);
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
            healthBar.gameObject.GetComponent<SliderController>().setColors(teamControlling, (float)health / maxHealth);
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
