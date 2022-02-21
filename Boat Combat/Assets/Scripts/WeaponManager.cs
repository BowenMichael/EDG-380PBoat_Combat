using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


namespace Com.BowenIvanov.BoatCombat
{
    public class WeaponManager : Photon.MonoBehaviour
    {
        #region Public Variables
        public string currentProjectile;//this is a string because when we instantiate the weapon, we can just call the name of the prefab

        #endregion

        #region Private Variables

        #endregion

        #region MonoBehavior Callbacks
        // Start is called before the first frame update
        void Awake()
        {
            currentProjectile = "testProjectile";//default

        }

        // Update is called once per frame
        void Update()
        {

        }

        #endregion

        #region Custom

        public void changeProjectile(string projectileName)
        {
            if(projectileName == null)
            {
                currentProjectile = "testProjectile";//just the default
                return;
            }
            currentProjectile = projectileName;
        }
        #endregion
    }
}
