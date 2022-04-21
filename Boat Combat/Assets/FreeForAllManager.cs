using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeForAllManager : MonoBehaviour
{
    [SerializeField] int elimLimit = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public int getElimLimit()
    {
        return elimLimit;
    }

    public void onElimLimitHit(int id)
    {
        ((GameObject)PhotonNetwork.player.TagObject).GetPhotonView().RPC("WinState", PhotonTargets.All, PhotonNetwork.player.ID);
    }

    [PunRPC]
    void onWin(int id)
    {
        
    }
}
