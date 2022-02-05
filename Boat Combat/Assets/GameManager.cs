using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Com.BowenIvanov.BoatCombat
{

    public class GameManager : MonoBehaviour
    {
        #region Private Variables

        private PlayerManager[] players;

        bool isSpawned = false;

        #endregion

        #region Monobehavior Callbacks

        // Start is called before the first frame update
        void Start()
        {
            
            
        }

        // Update is called once per frame
        void Update()
        {
            if (!isSpawned)
            {
                players = GameObject.FindObjectsOfType<PlayerManager>();
                if (players.GetLength(0) == 2)
                {
                    setSpawnPositions(); //Warning!!!: This Sets the position on all clients. this object should only be running on one but I'm not sure how to do that yet
                }
            }
        }

        #endregion

        #region Custom

        void setSpawnPositions()
        {
            isSpawned = true;
            players[0].transform.position = new Vector3(10, 1, 10);
            players[1].transform.position = new Vector3(-10, 1, -10);
        }

        #endregion
    }
}
