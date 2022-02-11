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
        private void Awake()
        {
            if (offlineMode)
            {
                PhotonNetwork.offlineMode = true;
                PhotonNetwork.JoinRandomRoom();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
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
