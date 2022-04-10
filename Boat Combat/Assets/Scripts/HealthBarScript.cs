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
