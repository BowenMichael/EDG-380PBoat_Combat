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

        string killStr;
        string damageStr;

        void Start()
        {
            killStr = Kills.text;
            damageStr = Damage.text;
        }

        private void Update()
        {
            updateText();
        }

        void updateText()
        {
            Kills.text = killStr + _kills;
            Damage.text = damageStr + _damage;
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
