using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Com.BowenIvanov.BoatCombat
{
    public class SettingsUI : MonoBehaviour
    {
        public GameObject settingUI;
        public GameObject winUI;
        public GameObject loseUI;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                settingUI.SetActive(!settingUI.activeSelf);
            }
        }

        public void win()
        {
            
            winUI.SetActive(true);
            StartCoroutine(endState());
        }

        public void lose()
        {
           
            winUI.SetActive(true);
            StartCoroutine(endState());
        }

        IEnumerator endState()
        {
            
            Time.timeScale = 0;

            yield return new WaitForSecondsRealtime(5f);
            GameManager.self.leaveRoom();
        }
    }
}