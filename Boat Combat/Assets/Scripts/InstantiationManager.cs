using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.BowenIvanov.BoatCombat
{
    public class InstantiationManager : MonoBehaviour
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

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

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Instatntiation Manager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
