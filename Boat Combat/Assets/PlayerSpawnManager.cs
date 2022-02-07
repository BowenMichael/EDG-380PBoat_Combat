using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerSpawnManager : PunBehaviour
    {
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        [Tooltip("list of spawn points"), SerializeField]
        public List<Transform> spawnPoints = new List<Transform>();

        public static PlayerSpawnManager self;

        int spawnIndex;

        public static void AddSpawnPoint(Transform transform)
        {
            //spawnPoints.Add(transform);

            //spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        //public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

        private void Start()
        {
            self = this;
        }

        /// <summary>
        /// Instantiates local player
        /// </summary>
        public void instantiatePlayer()
        {
            Vector3 spawnPosition = Vector3.zero;
            Quaternion spawnRotation = Quaternion.identity;

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
                    GameObject tmp = PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPosition, spawnRotation, 0);

                    
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }

        /// <summary>
        /// Gets the next spawn point based on the spawn index. Only to be run on master client
        /// </summary>
        /// <returns></returns>
        public Transform getSpawnPoint()
        {
            spawnIndex++;
            if (spawnPoints.Count > 0)
            {
                if (spawnIndex >= spawnPoints.Count)
                {
                    spawnIndex = 0;
                }
            }
            else
            {
                Debug.LogError("Player Spawn Error: No Spawn Points found");
                return null;
            }

            Debug.Log(string.Format("Found Spawn point at index {0}", spawnIndex));
            return spawnPoints[spawnIndex];
        }
        
    }
}
