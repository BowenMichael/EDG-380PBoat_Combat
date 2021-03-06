using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using Cinemachine;


namespace Com.BowenIvanov.BoatCombat
{
    public class testProjectileScript : Photon.MonoBehaviour
    {
        #region Public Variables

        public PhotonView sender;

        //public AudioSource projFiredSound;
        public AudioClip projImpactSound;

        #endregion

        #region Private Variables

        [SerializeField] float damage = 10f;
        private Rigidbody rb;

        #endregion

        #region Unity Events

        public UnityEvent<float> onDamage;
        public UnityEvent onKill;

        #endregion

        #region MonoBehavior Callbacks
        // Start is called before the first frame update
        void Awake()
        {
            //if (photonView.isMine)
            //{
            //    PhotonNetwork.Instantiate("testProjectile", new Vector3(0, 0, 0), Quaternion.identity, 0);
            //}

            //Vector3 front = gameObject.transform.right;
            //gameObject.GetComponent<Rigidbody>().AddForce(front * 100000 * Time.fixedDeltaTime);

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if(other.TryGetComponent(out HealthManager health))
                {
                    if (photonView.isMine)
                    {
                        health.gameObject.GetPhotonView().RPC("damageHeathWithPlayerInfo", PhotonTargets.AllViaServer, damage, sender.viewID);
                        //play impact sound
                        AudioSource audio = GetComponent<AudioSource>();
                        audio.clip = projImpactSound;
                        audio.Play();
                    }
                }
            }
            if (!other.CompareTag("CP"))
            {
                if (photonView.isMine)
                {
                    if (photonView.isRuntimeInstantiated)
                    {
                        PhotonNetwork.Destroy(gameObject);
                    }
                }
            }
                
        }

        #endregion
    }
}


