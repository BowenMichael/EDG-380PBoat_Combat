using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class PowerUpScript : Photon.MonoBehaviour
    {
        [SerializeField]private ParticleSystem part;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.gameObject.TryGetComponent(out testProjectileScript test))
            //{
            //    return;
            //}
            if (collision.gameObject.tag == "Player")
            {
                //change speed
                collision.gameObject.GetComponent<PlayerManager>().SpeedPowerUp();


                Instantiate(part, transform.position, transform.rotation);
                part.transform.position = transform.position;
                part.Play();
                //Destroy(gameObject, part.main.duration);

                //PhotonNetwork.Destroy(gameObject);

                //return;

                if (photonView.isMine)
                    if (photonView.isRuntimeInstantiated)
                        PhotonNetwork.Destroy(gameObject);

            }
        }
    }

}
