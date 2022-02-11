using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerSpawnManager : PunBehaviour
    {
        #region Public Variables

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        public int maxTeams;

        #endregion

        #region Static Variables

        [Tooltip("list of spawn points")]
        private static List<Transform> spawnPoints = new List<Transform>();

        public static PlayerSpawnManager self;

        #endregion

        #region Private Variables

        int spawnIndex = 0;
        int teamIndex = 0;

        #endregion

        #region Static Functions

        public static void AddSpawnPoint(Transform transform)
        {
            spawnPoints.Add(transform);

            spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
        }

        public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

        #endregion

        #region MonoBehavior Callbacks

        private void Start()
        {
            self = this;
        }

        #endregion

        #region Public Functions

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

        public int getTeam()
        {
            if(teamIndex == -1)
            {
                teamIndex = 1;
            }
            else if(teamIndex == 1)
            {
                teamIndex = -1;
            }
            else
            {
                teamIndex = -1;
                //Debug.LogError("Player Spawn Error: No Spawn Points found");
                //return -1;
            }

            Debug.Log(string.Format("Assigned team: {0}", teamIndex));
            return teamIndex;
        }

        #endregion

    }
}
