using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        string killStr;
        string damageStr;
        string speedStr;

        void Start()
        {
            killStr = Kills.text;
            damageStr = Damage.text;
            speedStr = Speed.text;
        }

        private void Update()
        {
            updateText();
        }

        void updateText()
        {
            Kills.text = killStr + _kills;
            Damage.text = damageStr + _damage;
            Speed.text = speedStr + (int)PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().getCurrentSpeed();
        }
        public void onDamage(float damage)
        {
            _damage += damage;
        }

        public void onKill()
        {
            _kills++;
        }
    }
}
