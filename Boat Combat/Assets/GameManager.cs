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
