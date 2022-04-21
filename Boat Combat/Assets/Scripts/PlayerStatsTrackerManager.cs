using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerStatsTrackerManager: MonoBehaviour
    {
        [SerializeField]int _kills = 0;
        [SerializeField]float _damage = 0;
        [SerializeField] Text Kills;
        [SerializeField] Text Damage;
        [SerializeField] Text Speed;
        [SerializeField] UnityEvent<int> onKillLimit = new UnityEvent<int>();
        private int elimLimit;
        private FreeForAllManager FFAM;


        string killStr;
        string damageStr;
        string speedStr;

        void Start()
        {
            killStr = Kills.text;
            damageStr = Damage.text;
            speedStr = Speed.text;
            FFAM = GameManager.self.GetComponent<FreeForAllManager>();
            elimLimit = FFAM.getElimLimit();
            onKillLimit.AddListener(FFAM.onElimLimitHit);

        }

        private void Update()
        {
            updateText();
        }

        void updateText()
        {
            if(Kills != null)
                Kills.text = killStr + _kills;

            if (Damage != null)
                Damage.text = damageStr + _damage;

            if (Speed != null)
                Speed.text = speedStr + (int)PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().getCurrentSpeed();
        }
        public void onDamage(float damage)
        {
            _damage += damage;
        }

        public void onKill()
        {
            _kills++;
            if(_kills >= elimLimit)
            {
                onKillLimit.Invoke(PhotonNetwork.player.ID);
            }
        }

        public int getKills()
        {
            return _kills;
        }

        public float getDamage()
        {
            return _damage;
        }
    }
}
