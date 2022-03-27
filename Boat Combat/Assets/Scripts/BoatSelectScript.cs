using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class BoatSelectScript : PunBehaviour
    {
        public Camera mc;
        public GameObject[] boats;




        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach(GameObject boat in boats)
            {
                boat.transform.Rotate(new Vector3(0.0f, 50.0f, 0.0f) * Time.deltaTime);
            }

            if(mc.transform.position.x == -20f)
            {
                PlayerPrefs.SetInt("BoatSelect", 2);//this will be the kayak 
            }

            if (mc.transform.position.x == 0f)
            {
                PlayerPrefs.SetInt("BoatSelect", 1);//this will be the normal boat 
            }

            if (mc.transform.position.x == 20f)
            {
                PlayerPrefs.SetInt("BoatSelect", 0);//this will be the speedboat 
            }

            //Debug.Log(PlayerPrefs.GetInt("BoatSelect"));

        }

        public void moveLeft()
        {
            if(mc.transform.position.x <= -20f)
            {
                return;
            }

            mc.transform.position = mc.transform.position + new Vector3(-20f, 0f, 0f);
        }

        public void moveRight()
        {
            if (mc.transform.position.x >= 20f)
            {
                return;
            }

            mc.transform.position = mc.transform.position + new Vector3(20f, 0f, 0f);

        }
    }
}
