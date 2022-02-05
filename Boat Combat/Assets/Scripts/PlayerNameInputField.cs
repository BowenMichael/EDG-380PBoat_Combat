using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Realtime;

namespace Com.BowenIvanov.BoatCombat
{
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : UnityEngine.MonoBehaviour
    {
        #region Private Constants

        //Store the PlayerPref Key to avoid typos
        const string PLAYER_NAME_PREF_KEY = "PlayerName";

        #endregion

        #region MonoBehavior Callbacks

        // Start is called before the first frame update
        void Start()
        {
            //If player already has a name
            string defaultName = string.Empty;
            InputField inputField = this.GetComponent<InputField>();
            if (inputField != null)
            {
                if (PlayerPrefs.HasKey(PLAYER_NAME_PREF_KEY))
                {
                    defaultName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
                    inputField.text = defaultName;
                }
            }

            PhotonNetwork.playerName = defaultName;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use to set the players username
        /// </summary>
        /// <param name="value">player new name</param>
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                if (!string.IsNullOrEmpty(PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY)))
                    return;

                Debug.LogError("player name is null or empty");
                return;
            }
            PhotonNetwork.playerName = value;

            PlayerPrefs.SetString(PLAYER_NAME_PREF_KEY, value);

        }

        #endregion
    }
}
