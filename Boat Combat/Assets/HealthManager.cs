using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class HealthManager : PunBehaviour, IPunObservable
    {
        [SerializeField] bool isDummy = false;
        private float currentHealth;
        [SerializeField] private float maxHealth = 100f;

        [SerializeField] Slider healthSlider;

        public UnityEvent onDeath;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(currentHealth);
            }
            else
            {
                currentHealth = (float)stream.ReceiveNext();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            onDeath.AddListener(GetComponent<PlayerManager>().respawn);
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

        public bool takeDamage(float value)
        {
            currentHealth -= value;
            return checkHealth();
        }
    }

}
