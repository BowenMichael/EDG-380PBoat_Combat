using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerManager : PunBehaviour//(for sending specific data)
    {
        #region Public Variables

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance; //Don't want to overuse this singlton

        [Tooltip("Each player needs to handle their own camera")]
        public GameObject cameraPrefab;


        public GameObject testProjectile;
        #endregion

        #region Private Variables

        [SerializeField]
        private CinemachineVirtualCamera cvCam;


        [Header("Attributes")]
        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed;


        [Header("Settings")]
        [SerializeField] bool isSliderControls;
        [SerializeField] float VertAccelSensitivity = 1.5f;
        [SerializeField] float HortAccelSensitivity = 1.5f;
        [SerializeField] float MaxAccelSensitivity = 5.0f;

        [Header("Mobile UI")]
        [SerializeField] private FixedJoystick mobileAxis;
        [SerializeField] private float mobileLookSpeed = 5f;
        [SerializeField] RectTransform mobileAxisRT;
        [SerializeField] RectTransform mobileRT;
        [SerializeField] Slider throttleSlider;
        [SerializeField] Slider steeringSlider;

        [Header("Readable info")]
        [SerializeField] private int team;
        [SerializeField] float horizontal;
        [SerializeField] float vertical;
        [SerializeField] private GameObject healthBarPrefab;





        [Header("Options")]
        [SerializeField] bool pauseInput;
        [SerializeField] bool isDragCamera;

        //Personal attached
        private Rigidbody rb;
        private HealthManager health;


        private Transform canvasTransform;
        private MobileManager mobileManager;
        private Transform spawnPoint;
        private int playerLookAtIndex = 0; //The index of the player being looked at by the local client
        private float rotHorizontal;
        Vector2 initalTouchPoint;

        //post processing
        private Volume volume;
        private Vignette vignette;

        #endregion

        #region Photon Callbacks



        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            info.sender.TagObject = this.gameObject;
        }

        #endregion

        #region MonoBehavior Callbacks

        private void Awake()
        {
            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
            if (photonView.isMine)
            {
                PlayerManager.LocalPlayerInstance = this.gameObject;
            }
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            //DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {

            if (PhotonNetwork.isMasterClient)
            {
                photonView.RPC("init", PhotonTargets.AllBuffered);
            }

            //set up Camera
            if (photonView.isMine)
            {
                
                if (cvCam == null)
                {
                    //GameObject tmp = Instantiate(cameraPrefab);
                    cvCam = FindObjectOfType<CinemachineVirtualCamera>();//tmp.GetComponent<CinemachineVirtualCamera>();
                }
                cvCam.Follow = transform;
                cvCam.LookAt = transform;
                rb = GetComponent<Rigidbody>();
                health = GetComponent<HealthManager>();
                

                //toggleCameraLookAt();
#if !(UNITY_ANDROID || UNITY_IOS)
                cvCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.m_InputAxisName = "Mouse X";
#endif
#if UNITY_ANDROID || UNITY_IOS
                
                mobileRT = FindObjectOfType<MobileManager>().getShoot();
                //mobileAxisRT = mobileAxis.transform.parent.GetComponentInParent<RectTransform>();
                mobileManager = FindObjectOfType<MobileManager>();
                throttleSlider = mobileManager.getThrottle();
                //steeringSlider = mobileManager.getSteering();
                
#endif
            }





            //setup postprocessing
            volume = FindObjectOfType<Volume>();
            if(volume.profile.TryGet<Vignette>(out vignette))
            {
                vignette.intensity.value = 0.0f;
            }
        }

        private void Update()
        {
            if (photonView.isMine)
            {
                ProcessInput();
            }

            
        }

        private void FixedUpdate()
        {
            if (photonView.isMine)
            {
                ProcessMovement();
                ProcessCameraMovement();
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            //if (collision.other.gameObject.tag == "Projectile")//damage taken
            //{
            //    if (collision.other.gameObject.name == "fastProjectile")//speedboat
            //    {
            //        takeDamage(5);
            //    }
            //    if (collision.other.gameObject.name == "testProjectile")//normal
            //    {
            //        takeDamage(10);
            //    }
            //    if (collision.other.gameObject.name == "cannonProjectile")//kayak
            //    {
            //        takeDamage(20);
            //    }
            //    else
            //    {
            //        takeDamage(10);//default
            //    }
            //}
        }

#endregion

        #region Public Methods

            #region Acessors

        public int getTeam() { return team; }

        public RectTransform getFireRT()
        {
            return mobileRT;
        }

        public bool getIsSliderControls() { return isSliderControls; }
        #endregion

            #region Mutators

        public void setIsSliderControls(bool sliderControls) 
        {
            isSliderControls = sliderControls;
            if (isSliderControls)
            {
                throttleSlider.gameObject.SetActive(true);
            }
            else
            {
                throttleSlider.gameObject.SetActive(false);
            }
        }

        public void setPausedInput(bool isPaused)
        {
            pauseInput = isPaused;
        }

        public bool setIsDragCamera(bool isDrag)
        {
            isDragCamera = isDrag;
            return isDragCamera;
        }

        public bool toggleIsDragCamera()
        {
            return setIsDragCamera(!isDragCamera);
            
        }
            #endregion

        /// <summary>
        /// Sets the hortAccelSensitivity which is a scalar for the max sensitivity
        /// </summary>
        /// <param name="multiple">value between 0-1</param>
        /// <returns></returns>
        public void setHortAccelSensitivity(float multiple)
        {
            HortAccelSensitivity = multiple;
        }

        /// <summary>
        /// Sets the VertAccelSensitivity which is a scalar for the max sensitivity
        /// </summary>
        /// <param name="multiple">value between 0-1</param>
        /// <returns></returns>
        public void setVertAccelSensitivity(float multiple)
        {
            VertAccelSensitivity = multiple;
        }

        #endregion

        #region Custom

        /// <summary>
        /// runs at update and updates player input
        /// </summary>
        void ProcessInput()
        {
            if (pauseInput) return;

#if !(UNITY_ANDROID || UNITY_IOS)
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

#else
            //Setting up tilt axis
            Vector3 tilt = Input.acceleration;
            tilt = Quaternion.Euler(90, 0, 0) * tilt;

            //Apply tilt to the horizontal axis
            horizontal = Mathf.Clamp(Input.acceleration.x * (HortAccelSensitivity * MaxAccelSensitivity), -1, 1);

            //If using slider controls
            if (!isSliderControls)
            {            
                vertical = Mathf.Clamp((Input.acceleration.y + .5f) * (VertAccelSensitivity * MaxAccelSensitivity), -1, 1);
                Debug.DrawRay(transform.position + Vector3.up, transform.worldToLocalMatrix * -new Vector3(vertical, 0.0f, horizontal), Color.cyan);

            }
            else
            {
                vertical = throttleSlider.value;

            }


            Touch[] ts = Input.touches;
            //Looping through touches on the screen
            for (int i = 0; i < ts.Length; i++)
            {
                if (isDragCamera)
                {
                    if (!mobileRT.rect.Contains(ts[i].position) &&
                        !throttleSlider.GetComponent<RectTransform>().rect.Contains(ts[i].position))
                    {
                        if (ts[i].phase == TouchPhase.Began)
                        {
                            initalTouchPoint = ts[i].position;
                        }
                        else if (ts[i].phase != TouchPhase.Began)
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
            }
#endif
            if (Input.GetKeyDown(KeyCode.I))
            {
                photonView.RPC("ChatMessage", PhotonTargets.All, photonView.owner.NickName, "I Message you");
            }
        }

        /// <summary>
        /// Runs at fixed update and updates player movement
        /// </summary>
        void ProcessMovement()
        {
            Vector3 direction = new Vector3(vertical, 0.0f, 0.0f);
            Vector3 force = transform.localToWorldMatrix * (direction * speed * Time.fixedDeltaTime);
            rb.AddForce(force);
            //Debug.Log(force);
           

            Vector3 rotation = new Vector3(0.0f, horizontal, 0.0f);
            gameObject.transform.Rotate(rotSpeed * rotation * Time.fixedDeltaTime);
        }

        void ProcessCameraMovement()
        {
            //found that this is all built into the camera already, we dont need to code it
            //if(cvCam == null)
            //{
            //    return;
            //}
            //Quaternion rotation = new Quaternion(0f, 30f, 0f, 0f);
            //cvCam.transform.rotation = rotation;
        }

        void toggleCameraLookAt()
        {
            PlayerManager[] players = GameManager.self.getPlayers();
            if (cvCam.LookAt == null) //init look at
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if (players[i] != this)
                    {
                        cvCam.LookAt = players[i].gameObject.transform;
                        return;
                    }
                }
            }
            else
            {
                if (players.Length > 2) 
                {
                    int j = playerLookAtIndex;
                    while (cvCam.LookAt == players[playerLookAtIndex].gameObject.transform && players[playerLookAtIndex] == this)
                    {
                        playerLookAtIndex++;
                        if(playerLookAtIndex >= players.Length)
                        {
                            playerLookAtIndex = 0;
                        }
                        if(playerLookAtIndex == j)
                        {
                            Debug.LogError("Camera Could not find a new Look at Target");
                            return;
                        }

                    }
                    cvCam.LookAt = players[playerLookAtIndex].gameObject.transform;
                }
            }
        }

        void setSpawnPoint()
        {
            Debug.Log("Setting spawn " + photonView.owner);
            spawnPoint = PlayerSpawnManager.self.getSpawnPoint();
            respawn();
        }

        public void respawn()
        {
            Debug.Log("Respawning " + photonView.owner);
            photonView.RPC("sendSpawnPoint", PhotonTargets.All, new float[] { spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z },
                                                                new float[] { spawnPoint.rotation.eulerAngles.x, spawnPoint.rotation.eulerAngles.y, spawnPoint.rotation.eulerAngles.z });
        }

        void setTeamByPlayerManager()
        {
            team = PlayerSpawnManager.self.getTeam();
            photonView.RPC("sendTeam", PhotonTargets.AllBuffered, team);
        }

        public void SpeedPowerUp()
        {
            StartCoroutine(BoostSpeed());
        }

        IEnumerator BoostSpeed()//speed doubled for 5 seconds
        {
            Debug.Log("Speed Boosted");
            speed *= 2;
            yield return new WaitForSeconds(5);
            speed /= 2;
        }


        void checkHealth()
        {
            if (currentHealth <= 0)
            {
                photonView.RPC("onDeath", PhotonTargets.All);
            }
        }

        public void takeDamage(int value)
        {
            currentHealth -= value;
            StartCoroutine(DamageVignette());
        }

        IEnumerator DamageVignette()
        {
            vignette.intensity.value = 0.3f;
            yield return new WaitForSeconds(2);
            float t = 0f;
            while (t <= 1.0)
            {
                t += Time.deltaTime / 100f;
                vignette.intensity.value = Mathf.Lerp(0.3f, 0.0f, Mathf.SmoothStep(0f, 1f, t));

                //yield return null;
            }


            //vignette.intensity.value = 0.0f;

        }

        #endregion

        #region Custom RPC

        /// <summary>
        /// RPC Function that runs init for players. Only to run on the master client
        /// </summary>
        [PunRPC]
        void init()
        {
            //Set Team
            setTeamByPlayerManager();

            //SetSpawnPoint
            setSpawnPoint();


        }


        [PunRPC]
        void ChatMessage(string name, string message)
        {
            Debug.Log(string.Format("ChatMessage: {0} {1}", name, message));
        }

        [PunRPC]
        void setPosition(float[] pos)
        {
            transform.position = new Vector3(pos[0], pos[1], pos[2]);
        }

        [PunRPC]
        void sendSpawnPoint(float[] pos, float[] rot)
        {
            transform.position = new Vector3(pos[0], pos[1], pos[2]);
            transform.rotation = Quaternion.Euler(new Vector3(rot[0], rot[1], rot[2]));
        }

        [PunRPC]
        void sendTeam(int t)
        {
            team = t;
        }

        [PunRPC]
        void EndState(int winningTeam)
        {
              GameManager.self.loadScene(winningTeam);
        }

#endregion
    }
}
