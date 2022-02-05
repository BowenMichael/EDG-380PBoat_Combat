using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon;

namespace Com.BowenIvanov.BoatCombat
{
    /// <summary>
    /// Author: Michael Bowen Feb 5 2022
    /// Use to navigate easily with unity button GUI
    /// </summary>
    public class SceneNavigator : PunBehaviour
    {
        #region Public Variables

        public string launcherScene;


        #endregion

        #region Public Methods

        /// <summary>
        /// Use this function in button onClick and drag scene asset from files
        /// </summary>
        /// <param name="scene"></param>
        public void loadScene(string scene)
        {
            SceneManager.LoadScene(scene); // don't need to check if it is in build order because unity has a proper debug message for this problem
        }

        #endregion

        #region Photon Callbacks

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene
        /// </summary>
        public override void OnLeftRoom()
        {
            if (!string.IsNullOrEmpty(launcherScene))
                SceneManager.LoadScene(launcherScene);
            else
                SceneManager.LoadScene(0);
        }
       
        #endregion
    }
}
