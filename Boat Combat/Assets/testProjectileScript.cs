using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


namespace Com.BowenIvanov.BoatCombat
{
    public class testProjectileScript : Photon.MonoBehaviour
    {
        #region Public Variables


        #endregion

        #region Private Variables

        private Rigidbody rb;

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

        private void OnCollisionEnter(Collision collision)
        {
            //Object.Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
        }

        #endregion
    }
}


