using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class NetworkManager : PunBehaviour
    {
        #region Public Variables



        #endregion

        #region Poton Callbacks

        public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
        {
            Debug.LogFormat("OnPLayerEnteredRoom() {0}", newPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.isMasterClient);

                LoadArena();
            }
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            Debug.LogFormat("OnPLayerLeftRoom() {0}", otherPlayer.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.isMasterClient);

                LoadArena();
            }
        }

        

        #endregion

        #region Public Methods

        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            
        }

        #endregion

        #region Private Methods

        private void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);

        }

        #endregion
    }
}
