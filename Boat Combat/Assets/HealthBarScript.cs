using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class HealthBarScript : MonoBehaviour
    {
        private GameObject player;
        public Vector3 offset;
        public float startScale;
        public float MaxDistance;
        Vector2 sizeDelta;
        public Slider fill;
        public GameObject background;

        private string playerName;
        public Text username;


        private void Start()
        {
            sizeDelta = transform.GetComponent<RectTransform>().sizeDelta * startScale;
            fill = GetComponent<Slider>();
        }
        private void Update()
        {
            if(player == null)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                    return;
                player = PlayerManager.LocalPlayerInstance;
            }


            Vector3 diff = (player.transform.position - Camera.main.transform.position);
            Vector2 playerInScreenSpace = Camera.main.WorldToScreenPoint(player.transform.position);


            //if (!Camera.main.rect.Contains(-playerInScreenSpace)) return;

            float distance = diff.magnitude;
            if (distance > MaxDistance)
            {
                fill.fillRect.gameObject.SetActive(false);
                background.SetActive(false);
                username.gameObject.SetActive(false);
                return;
            }

            fill.fillRect.gameObject.SetActive(true);
            background.SetActive(true);
            username.gameObject.SetActive(true);
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Camera.main.transform.position);
            //transform.LookAt(Camera.main.transform.position);
            float inverseDistance = 1.0f / distance;
            transform.position = new Vector3(playerInScreenSpace.x, playerInScreenSpace.y, 0.0f) + offset;// * startScale * inverseDistance;
            transform.GetComponent<RectTransform>().sizeDelta = sizeDelta * inverseDistance;
            //transform.Rotate(0, 180, 0);
        }

        public void setPlayer(GameObject plr)
        {
            player = plr;
            playerName = plr.GetPhotonView().owner.NickName;
            if (playerName != null)
                username.text = playerName;
        }
    }
}
