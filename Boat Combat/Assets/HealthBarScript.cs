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
        private void Start()
        {
            sizeDelta = transform.GetComponent<RectTransform>().sizeDelta * startScale;
            fill = GetComponent<Slider>();
        }
        private void FixedUpdate()
        {
            


            if(player == null)
            {
                if (PlayerManager.LocalPlayerInstance == null)
                    return;
                player = PlayerManager.LocalPlayerInstance;
            }

            float distance = Vector3.Distance(player.transform.position, Camera.main.transform.position);
            if (distance > MaxDistance)
            {
                fill.fillRect.gameObject.SetActive(false);
                background.SetActive(false);
                return;
            }

            fill.fillRect.gameObject.SetActive(true);
            background.SetActive(true);
            //Vector3 pos = Camera.main.ScreenToWorldPoint(Camera.main.transform.position);
            //transform.LookAt(Camera.main.transform.position);
            Vector3 playerInScreenSpace = Camera.main.WorldToScreenPoint(player.transform.position);
            transform.position = new Vector3(playerInScreenSpace.x, playerInScreenSpace.y, Camera.main.nearClipPlane + .1f) + offset * startScale * (1/distance);
            transform.GetComponent<RectTransform>().sizeDelta = sizeDelta *  1/distance;
            //transform.Rotate(0, 180, 0);
        }
    }
}
