using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Com.BowenIvanov.BoatCombat;

public class Debug_Manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            for (int i = 0; i < 20; i++)
            {
                PlayerManager.LocalPlayerInstance.GetComponent<PlayerStatsTrackerManager>().onKill();
            }
        }
    }
}
