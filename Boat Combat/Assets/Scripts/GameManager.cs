using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;



namespace Com.BowenIvanov.BoatCombat
{

    public class GameManager : Photon.MonoBehaviour
    {


        #region Private Variables

        [SerializeField] TMP_Text StartText;
        [SerializeField] TMP_Text timerText;

        [SerializeField] int secondsForStartSequence;
        [SerializeField] int secondsForMatch;
        [SerializeField] float matchTime;
        [SerializeField] bool skipStart;
        [SerializeField] bool isFFA;
        bool isStarted = false;

        public ControlPointManager cpm;

        float startTime;
        float gameStartTime;
        float gameEndTime;

        NetworkManager nm;

        [SerializeField] PlayerManager[] plrs;
        [SerializeField] SettingsUI UI;

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
            nm = GetComponent<NetworkManager>();
            if (PhotonNetwork.offlineMode)
            {
                skipStart = true;
            }
            StartCoroutine(checkForStart());
        }

        private void Update()
        {
            

        }

        #endregion

        #region Public Methods

        public void leaveRoom()
        {
            nm.LeaveRoom();
        }

        public void WinState(int winningTeam)
        {
            
            ((GameObject)PhotonNetwork.masterClient.TagObject).GetPhotonView().RPC("EndState", PhotonTargets.All, winningTeam);
        }

        public void loadScene(int winner)
        {
            if (winner == PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().getTeam())
            {
                UI.win();
            }
            else
            {
                UI.lose();
            }
        }


        public void StartCountDown()
        {

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

        IEnumerator checkForStart()
        {
            if (PlayerPrefs.GetInt("isTwoPlayer") == 1)
            {
                if (PhotonNetwork.room.PlayerCount == 2)
                {
                    isFFA = false;
                    StartCountDown();
                    PhotonNetwork.room.IsOpen = false;
                    yield return null;

                }
                else
                {
                    StartText.text = "Waiting for players: " + PhotonNetwork.room.PlayerCount + "/2";
                    yield return new WaitForSecondsRealtime(.25f);
                    Debug.Log("Check Start");
                    StartCoroutine(checkForStart());
                }
            }
            else
            {
                if(PhotonNetwork.room.PlayerCount >= 3)
                {
                    isFFA = true;
                    if(!isStarted)
                        StartCountDown();
                    yield return null;
                }
                else
                {
                    StartText.text = "Waiting for players: " + PhotonNetwork.room.PlayerCount + "/3";
                    yield return new WaitForSecondsRealtime(.25f);
                    Debug.Log("Check Start");
                    StartCoroutine(checkForStart());
                }
            }

            

        }
        IEnumerator startSequence()
        {
            for(int i = secondsForStartSequence; i > 0; i--)
            {
                StartText.text = string.Format("Game Starts in \n {0}", i);
                yield return new WaitForSecondsRealtime(1);
            }
            photonView.RPC("start", PhotonTargets.AllBuffered);
           

        }

        [PunRPC]
        [ContextMenu("Dev Start")]
        void start()
        {
            StartText.text = "GO!";
            Time.timeScale = 1;
            StartText.CrossFadeAlpha(0, 1, false);
            if (PhotonNetwork.isMasterClient)
            {
                StartCoroutine(matchTimer());
            }
            isStarted = true;
            plrs = FindObjectsOfType<PlayerManager>();
        }

        IEnumerator matchTimer()
        {
            for (int i = secondsForMatch; i > 0; i--)
            {
                photonView.RPC("sendTimer", PhotonTargets.AllViaServer, i);
                yield return new WaitForSecondsRealtime(1);
            }
            photonView.RPC("endGameByMatchTime", PhotonTargets.All);
        }

        #endregion

        #region RPC

        [PunRPC]
        void sendTimer(int i)
        {
            timerText.text = string.Format("Time Left: {0}:{1}", i / 60, i % 60);
        }

        [PunRPC]
        void endGameByMatchTime()
        {
            if(!isFFA)
                loadScene(cpm.getWinning());
            else
            {
                UI.win();
            }
        }

        [PunRPC]
        void initLocalPlayer()
        {
            //if(PlayerManager.LocalPlayerInstance != null)
            //PlayerManager.LocalPlayerInstance.GetPhotonView().RPC("init", PhotonTargets.All);
        }


        #endregion
    }
}
