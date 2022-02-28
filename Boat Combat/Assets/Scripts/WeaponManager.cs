using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


namespace Com.BowenIvanov.BoatCombat
{
    public class WeaponManager : Photon.MonoBehaviour
    {
        #region Public Variables
        public string currentProjectile;//this is a string because when we instantiate the weapon, we can just call the name of the prefab
                                        //public GameObject player;

        [SerializeField] private float projSpeed;
        [SerializeField] private float speed;
        [SerializeField] private int numProjectiles;

        public GameObject testProjectile;

        #endregion

        #region Private Variables


        private float horizontal;
        private float vertical;

        #endregion

        #region MonoBehavior Callbacks
        // Start is called before the first frame update
        void Awake()
        {
            currentProjectile = "testProjectile";//default

        }

        // Update is called once per frame
        void Update()
        {


            if (photonView.isMine)
            {
                ProcessInput();
            }


        }

        #endregion

        #region Custom

       

        public void changeProjectile(string projectileName)
        {
            if(projectileName == null)
            {
                currentProjectile = "testProjectile";//just the default
                return;
            }
            currentProjectile = projectileName;
        }

        void ProcessInput()
        {
#if !UNITY_ANDROID
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            //fire projectile
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                resetShot();
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0))
            {
                chargeShot();
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
            {
                fireProjectile();
            }

#else
            horizontal = -mobileAxis.Horizontal;
            vertical = mobileAxis.Vertical;

            //Camera Controls
            Touch[] ts = Input.touches;


            for (int i = 0; i < ts.Length; i++)
            {
                if ((ts[i].position.x < mobileRT.position.x + mobileRT.rect.width * .5f && ts[i].position.y < mobileRT.position.y + mobileRT.rect.height * .5f))
                {
                    //Debug.Log("Fire area: " + ts[i].position);
                    if (ts[i].phase == TouchPhase.Began)
                    {
                        resetShot();
                    }
                    else if (ts[i].phase == TouchPhase.Stationary || ts[i].phase == TouchPhase.Moved)
                    {
                        chargeShot();
                    }
                    else if (ts[i].phase == TouchPhase.Ended)
                    {
                        fireProjectile();
                    }
                    i = ts.Length;
                    break;
                }
                else if((ts[i].position.x < mobileAxisRT.position.x - mobileAxisRT.rect.width * .5f && ts[i].position.y > mobileAxisRT.position.y + mobileAxisRT.rect.height * .5f))
                {
                    if(ts[i].phase == TouchPhase.Began)
                    {
                        initalTouchPoint = ts[i].position;
                    }
                    else if(ts[i].phase == TouchPhase.Moved)
                    {
                        CinemachineOrbitalTransposer transposer = cvCam.GetCinemachineComponent<CinemachineOrbitalTransposer>();
                        Vector2 diff = ts[i].position - initalTouchPoint;
                        float distance = Vector2.Dot(diff, Vector2.right);
                        //distance = Mathf.Clamp(distance, transposer.m_XAxis.m_MinValue, transposer.m_XAxis.m_MaxValue);
                        //Debug.Log("MoveCamera: " + distance);
                        
                        transposer.m_XAxis.Value += (distance) * mobileLookSpeed * Time.deltaTime;
                        //Input.simulateMouseWithTouches = true;// cvCam.GetInputAxisProvider();
                        Debug.DrawRay(initalTouchPoint, diff, Color.red, 1.0f);
                    }
                    
                }
            }
#endif
        }

        void resetShot()
        {
            projSpeed = 10000;
        }

        void chargeShot()
        {
            projSpeed = (projSpeed + 150);
        }

        void fireProjectile()
        {
            Vector3 boatPosition = gameObject.transform.position;
            Quaternion boatRotation = gameObject.transform.rotation;
            //GameObject proj = GameObject.Instantiate(testProjectile);
            //using PhotonNetwork.Instantiate the created game object is set up for the network
            for (int i = 0; i < numProjectiles; i++)
            {
                float angleModifier = (i / numProjectiles) * 2f -1f;
                //angleModifier *= projectilesSpreadAngle;
                GameObject proj = null;
                if (PhotonNetwork.inRoom)
                {
                    proj = PhotonNetwork.Instantiate(currentProjectile, boatPosition, boatRotation, 0);
                }

                if (proj == null)
                {
                    return;
                }
                Vector3 cameraDirection = (Camera.main.transform.position - gameObject.transform.position).normalized;
                proj.transform.forward = cameraDirection;
                proj.transform.position = new Vector3(boatPosition.x + angleModifier * i * -cameraDirection.z + (2f * i), boatPosition.y + 1f, boatPosition.z + angleModifier * i * cameraDirection.x);
                proj.transform.position += (proj.transform.forward * 3);

                Vector3 front = gameObject.transform.right;

                Vector3 projectileDirection = new Vector3(-cameraDirection.x, 0, -cameraDirection.z).normalized;

                proj.gameObject.GetComponent<Rigidbody>().AddForce(projectileDirection * (projSpeed + speed + 1000) * Time.fixedDeltaTime);
            }
        }

        

        #endregion

        #region Acessors
        public float getProjSpeed() { return projSpeed; }

        #endregion

    }




}
