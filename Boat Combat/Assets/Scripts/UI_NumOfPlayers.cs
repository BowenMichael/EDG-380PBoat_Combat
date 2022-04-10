using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_NumOfPlayers : MonoBehaviour
{
    Text numOfPlayerText;
    string numOfPlayersStr;
    // Start is called before the first frame update
    void Start()
    {
        numOfPlayerText = GetComponent<Text>();
        numOfPlayersStr = numOfPlayerText.text;
    }

    // Update is called once per frame
    void Update()
    {
        numOfPlayerText.text = numOfPlayersStr + " (" + PhotonNetwork.room.PlayerCount + "/" + PhotonNetwork.room.MaxPlayers + ")";
    }
}
