using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class ControlPointManager : MonoBehaviour, IPunObservable
    {
        [Tooltip("0: Neutral")]
        [SerializeField] int teamControlling = 0;
        [SerializeField] int maxHealth = 100;
        [SerializeField] float updateHealthPerSecond;
        [SerializeField] List<CapturePoint> points = new List<CapturePoint>();

        int health = 0;

        int teamContesting = 0;

        float timer = 0;

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
            foreach (CapturePoint pm in points)
            {
                if (pm.isControllingAtMaxHealth())
                {
                    teamContesting += pm.getControlling();
                }
            }

            if(teamContesting > 1)
            {
                teamContesting = 1;
            }
            else if(teamContesting < -1)
            {
                teamContesting = -1;
            }

            if (teamContesting == 0)
            {
                return;
            }
            else if (teamControlling == teamContesting)
            {
                health++;
                if (health == maxHealth)
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

    }
}
