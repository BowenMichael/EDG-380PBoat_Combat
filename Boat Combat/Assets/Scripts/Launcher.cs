using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class Launcher : PunBehaviour
    {
        public const int NUM_PLAYER_PER_ROOM = 4; //could read this into game from network config
        #region Private Serializable Fields

        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        private byte maxPlayersPerRoom = NUM_PLAYER_PER_ROOM;

        [Tooltip("The Ui Panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        [Tooltip("The UI Label to inform the user that they are connected")]
        [SerializeField]
        private GameObject connectedLabel;



        #endregion

        #region Private Fields

        string gameVersion = "1"; //This client's version number. Users are separated from each other by gameVersion which allows you to make breaking changes

        bool isConnecting; //Keeps track of the process. Since connection is asynchronous and is based on several call backs from photon. we need to keep track of this to properly adjust the behavior when we recieve call back by photon. Typically this is used for onConnectedToMaster() callback

        bool _isTwoPlayer = true;

        #endregion

        #region PunCallbacks Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");

            // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
        }

        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);

            isConnecting = false;

            Debug.Log("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

            //Failed to join Random lobby none exist or all are full so creating a new one
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            if (_isTwoPlayer)
            {
                if (PhotonNetwork.room.PlayerCount == 1)
                {
                    Debug.Log("We load the 'Room for 1'");

                    PhotonNetwork.LoadLevel("Room1v1");
                }
            }
            else
            {
                if (PhotonNetwork.room.PlayerCount == 1)
                {
                    Debug.Log("We load the 'Room for 3'");

                    PhotonNetwork.LoadLevel("RoomFFA");
                }
            }
            //Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
            progressLabel.SetActive(false);
            connectedLabel.SetActive(true);
        }

        #endregion

        #region MonoBehavior Callbacks

        private void Awake()
        {
            //This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            //connect();

            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            connectedLabel.SetActive(false);

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process
        /// -If already connected Join a random room
        /// -If not yet connected, connect this applicaiton instance to Photon Cloud Network
        /// </summary>
        public void connect(bool isTwoPlayer)
        {
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);

            this._isTwoPlayer = isTwoPlayer;

            if(isTwoPlayer)
                PlayerPrefs.SetInt("isTwoPlayer", 1);
            else
                PlayerPrefs.SetInt("isTwoPlayer", 0);

            if (PhotonNetwork.connected)
            {
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings(gameVersion);
                PhotonNetwork.gameVersion = gameVersion;
            }
        }

        #endregion
    }
}

