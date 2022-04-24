using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Com.BowenIvanov.BoatCombat;

public class UI_LeaderManager : MonoBehaviour
{
    TMP_Text leaderText;
    string leaderStr;
    PlayerStatsTrackerManager leader;

    private void Start()
    {
        leaderText = GetComponent<TMP_Text>();
        leaderStr = leaderText.text;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.self.getIsStarted())
        {
            leader = GameManager.self.getKillLeader();
            leaderText.text = leaderStr + leader.gameObject.GetPhotonView().owner.NickName +"\nElims:"+ leader.getKills();
        }
    }
}
