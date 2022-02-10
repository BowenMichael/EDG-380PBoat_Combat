using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;



namespace Com.BowenIvanov.BoatCombat
{

    public class GameManager : MonoBehaviour
    {


        #region Private Variables

        [SerializeField] TMP_Text StartText;

        [SerializeField] int secondsForStartSequence;
        [SerializeField] float matchTime;
        [SerializeField] bool skipStart;

        float startTime;
        float gameStartTime;
        float gameEndTime;

        [SerializeField] PlayerManager[] plrs;

        #endregion

        public static GameManager self;

        #region Monobehavior Callbacks

        private void Awake()
        {
            self = this;
            Time.timeScale = 0;

           
        }

        private void Start()
        {
            if (PhotonNetwork.offlineMode)
            {
                skipStart = true;
            }
        }

        private void Update()
        {
            
        }

        #endregion

        #region Public Methods

        public void WinState()
        {
            
        }

        public void StartCountDown()
        {
            plrs = FindObjectsOfType<PlayerManager>();
            if (!skipStart) 
            { 
                StartCoroutine(startSequence());
            }
            else
            {
                StartText.CrossFadeAlpha(0, 1, false);
                Time.timeScale = 1;
            }
            //startTime = Time.time;
            //gameStartTime = Time.time + secondsForStartSequence;



        }

        public PlayerManager[] getPlayers()
        {
            return plrs;
        }

        #endregion

        #region Custom

        IEnumerator startSequence()
        {
            for(int i = secondsForStartSequence; i > 0; i--)
            {
                StartText.text = string.Format("Game Starts in \n {0}", i);
                yield return new WaitForSecondsRealtime(1);
            }
            StartText.text = "GO!";
            Time.timeScale = 1;
            StartText.CrossFadeAlpha(0, 1, false);

        }

        #endregion
    }
}
