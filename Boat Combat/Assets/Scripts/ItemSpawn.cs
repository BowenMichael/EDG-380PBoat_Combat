using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.BowenIvanov.BoatCombat
{
    public class ItemSpawn : Photon.MonoBehaviour
    {
        //items to spawn
        //[SerializeField] private GameObject trap;
        [SerializeField] private int numOfTraps;

        //[SerializeField] private GameObject powerUp;
        [SerializeField]private int numOfPowerUps;

        //dimentions to spawn objects
        //private Vector3 BLCorner = new Vector3(-100f, 0f, 100f);
        //private Vector3 BRCorner = new Vector3(300f, 0f, 100f);
        //private Vector3 TLCorner = new Vector3(-100f, 0f, 700f);
        //private Vector3 TRCorner = new Vector3(300f, 0f, 700f);


        // Start is called before the first frame update
        void Start()
        {
            for(int i = 1; i <= numOfTraps; i++)
            {
                GameObject item = null;
                float x = Random.Range(-100f, 300f);
                //float x = Random.Range(0f, 5f);
                float y = 1f;
                float z = Random.Range(100f, 700f);
                //float z = Random.Range(0f, 5f);

                Vector3 randPos = new Vector3(x, y, z);

                if (PhotonNetwork.inRoom)
                {
                    item = PhotonNetwork.Instantiate("Trap(Mine)", randPos, this.transform.rotation, 0);
                }

                if(item == null)
                {
                    return;
                }
            }

            for (int i = 1; i <= numOfPowerUps; i++)
            {
                GameObject item = null;
                float x = Random.Range(-100f, 300f);
                //float x = Random.Range(0f, 5f);
                float y = 1f;
                float z = Random.Range(100f, 700f);
                //float z = Random.Range(0f, 5f);

                Vector3 randPos = new Vector3(x, y, z);

                if (PhotonNetwork.inRoom)
                {
                    item = PhotonNetwork.Instantiate("PowerUp", randPos, this.transform.rotation, 0);
                }

                if (item == null)
                {
                    return;
                }

            }

        }

        // Update is called once per frame
        void Update()
        {

        }

    
    }

}