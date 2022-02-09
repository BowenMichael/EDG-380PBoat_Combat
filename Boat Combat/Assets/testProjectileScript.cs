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
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            //if (photonView.isMine)
            //{
            //    testProjectileScript.LocalPlayerInstance = this.gameObject;
            //}
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            //DontDestroyOnLoad(this.gameObject);

            //Vector3 front = gameObject.transform.right;
            //gameObject.GetComponent<Rigidbody>().AddForce(front * 100000 * Time.fixedDeltaTime);

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnCollisionEnter(Collision collision)
        {
            Object.Destroy(gameObject);
        }

        #endregion
    }
}


