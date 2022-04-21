using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.BowenIvanov.BoatCombat
{
    public class SetFowardTowardPlayer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(PlayerManager.LocalPlayerInstance != null)
            {
                transform.forward = (transform.position - Camera.main.transform.position).normalized;
            }
        }
    }
}
