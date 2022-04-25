using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disconnect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.Disconnect();
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
