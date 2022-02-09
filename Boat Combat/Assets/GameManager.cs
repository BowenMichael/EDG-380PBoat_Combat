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

        float startTime;
        float gameStartTime;
        float gameEndTime;

        #endregion

        public static GameManager self;

        #region Monobehavior Callbacks

        private void Awake()
        {
            self = this;
            Time.timeScale = 0;
        }

        private void Update()
        {
            
        }

        #endregion

        #region Public Methods

        public void StartCountDown()
        {
            StartCoroutine(startSequence());
            //startTime = Time.time;
            //gameStartTime = Time.time + secondsForStartSequence;



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
