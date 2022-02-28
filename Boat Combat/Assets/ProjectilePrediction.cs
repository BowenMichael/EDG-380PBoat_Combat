using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Com.BowenIvanov.BoatCombat {
    public class ProjectilePrediction : MonoBehaviour
    {
        public float timeIterations = 10;
        public GameObject indicator;

        Transform parent;
        private WeaponManager plrM;
        GameObject aim;
        // Start is called before the first frame update
        void Start()
        {
            plrM = GetComponent<WeaponManager>();
            //parent = Instantiate(new GameObject()).transform;
            if (plrM.photonView.isMine)
            {
                aim = Instantiate(indicator);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (plrM.photonView.isMine)
            {
                waterTarget();
            }
            //projectArc();
        }

        void waterTarget()
        {
            if(aim == null)
            {
                return;
            }
            Vector3 pos = gameObject.transform.position;
            pos += new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 cameraDirection = (Camera.main.transform.position - gameObject.transform.position).normalized;
            Vector3 projectileDirection = new Vector3(-cameraDirection.x, cameraDirection.y, -cameraDirection.z).normalized;
            float speed = ((plrM.getProjSpeed())/plrM.testProjectile.GetComponent<Rigidbody>().mass) * Time.fixedDeltaTime;
            Vector3 v = speed * projectileDirection * Time.fixedDeltaTime;
            //Debug.Log(v);
            for (int t = 0; t < 1000; t++)
            {
                float time = t * Time.fixedDeltaTime;
                float yPos = pos.y + v.y * time + .5f * -9.8f * time * time;
                //Debug.Log(yPos);
                if (yPos < 0)//xPos < ground plane
                {
                    //Debug.Log("Set Aim pos" + t);
                    float xPos = pos.x + v.x * time;
                    float zPos = pos.z + v.z * time;
                    aim.transform.position = new Vector3(xPos, 1.0f, zPos);
                    return;
                }

            }
        }

       

        
    }
}
