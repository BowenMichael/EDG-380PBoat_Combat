using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class TrapScript : Photon.MonoBehaviour
    {
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
                collision.gameObject.GetPhotonView().RPC("takeDamage", PhotonTargets.AllViaServer, 25);
                PhotonNetwork.Destroy(gameObject);

                if (photonView.isMine)
                    if (photonView.isRuntimeInstantiated)
                        PhotonNetwork.Destroy(gameObject);


                //return;
            }
        }
    }

}
