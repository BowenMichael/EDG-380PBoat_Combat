using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


namespace Com.BowenIvanov.BoatCombat
{
    public class SettingsUI : MonoBehaviour
    {
        public GameObject settingUI;
        public GameObject winUI;
        public GameObject loseUI;
        public TMP_Text timerText;

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                settingUI.SetActive(!settingUI.activeSelf);
                Cursor.visible = settingUI.activeSelf;

            }
        }

        public void win()
        {
            
            winUI.SetActive(true);
            StartCoroutine(endState());
        }

        public void lose()
        {
           
            loseUI.SetActive(true);
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
