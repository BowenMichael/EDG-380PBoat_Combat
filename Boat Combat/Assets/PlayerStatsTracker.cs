using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerStatsTracker : MonoBehaviour
    {
        int _kills = 0;
        float _damage = 0;

        

        void onDamage(float damage)
        {
            _damage += damage;
        }

        void onKill()
        {
            _kills++;
        }
    }
}
