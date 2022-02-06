using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using ExitGames.Client.Photon;


namespace Com.BowenIvanov.BoatCombat
{
    public class InstantiationManager : MonoBehaviour
    {

        [Tooltip("For testing objects outside a network enviorment")]
        public bool offlineMode;

        // Start is called before the first frame update
        void Start()
        {
            if (offlineMode)
            {
                PhotonNetwork.offlineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }

            
            GetComponent<PlayerSpawnManager>().instantiatePlayer();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Custom

        



        #endregion
    }
}
