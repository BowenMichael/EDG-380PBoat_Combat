using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class HealthManager : PunBehaviour
    {
        [SerializeField] bool isDummy = false;
        private float currentHealth;
        [SerializeField] private float maxHealth = 100f;

        [SerializeField] Slider healthSlider;

        public UnityEvent onDeath;
        public UnityEvent<float> onDamaged;



        // Start is called before the first frame update
        void Start()
        {
            onDeath.AddListener(GetComponent<PlayerManager>().respawn);
            onDamaged.AddListener(GetComponent<PlayerManager>().onDamaged);
            //set current health to max health
            currentHealth = maxHealth;

        }

        // Update is called once per frame
        void Update()
        {
            if (photonView.isMine)
            {
                checkHealth();

            }


            //need to make this part of the photon view eventually
            //this updates the visual healthbar
            healthSlider.value = currentHealth / maxHealth;
        }



        [PunRPC]
        private void processDeath()
        {
            if(!isDummy)
            onDeath.Invoke();
            currentHealth = maxHealth;
        }

        public bool checkHealth()
        {
            if (currentHealth <= 0)
            {
                photonView.RPC("processDeath", PhotonTargets.All);
                return true;
            }
            return false;
        }

        [PunRPC]
        public bool takeDamage(float value)
        {
            onDamaged.Invoke(value);
            currentHealth -= value;
            return checkHealth();
        }

        [PunRPC]
        public void damageHeathWithPlayerInfo(float value, int senderID)
        {
            PhotonView sender = null;
            foreach(PlayerManager plr in GameManager.self.getPlayers())
            {
                if(plr.gameObject.GetPhotonView().viewID == senderID)
                {
                    sender = plr.gameObject.GetPhotonView();
                    break;
                }
            }

            if (sender == null) return;

            PlayerStatsTrackerManager pstm = sender.gameObject.GetComponent<PlayerStatsTrackerManager>();
            if (takeDamage(value))
            {
                pstm.onKill();
            }
            pstm.onDamage(value);
             
        }

        public float getCurrentHealth()
        {
            return currentHealth;
        }
    }

}
