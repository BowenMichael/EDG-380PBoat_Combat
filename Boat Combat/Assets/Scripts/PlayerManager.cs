using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;
using UnityEngine.UI;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerManager : PunBehaviour, IPunObservable//(for sending specific data)
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

        private Rigidbody rb;

        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed;
        [SerializeField] private float sensitivity;
        [SerializeField] private int team;
        [SerializeField] private float projSpeed;
        [SerializeField] private float numProjectiles;
        [SerializeField] private float projectilesSpreadAngle;


        private float horizontal;
        private float vertical;
        private int playerLookAtIndex = 0; //The index of the player being looked at by the local client


        private float rotHorizontal;

        [SerializeField] private GameObject healthBarPrefab;
        private Transform canvasTransform;
        private Slider healthSlider;
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private Transform spawnPoint;

        [SerializeField] private FixedJoystick mobileAxis;
        [SerializeField] private float mobileLookSpeed = 5f;
        [SerializeField] RectTransform mobileAxisRT;
        [SerializeField]RectTransform mobileRT;

        Vector2 initalTouchPoint;

        #endregion

        #region Photon Callbacks

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(currentHealth);
            }
            else
            {
                currentHealth = (float)stream.ReceiveNext();
            }
        }

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
            DontDestroyOnLoad(this.gameObject);
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
                    GameObject tmp = Instantiate(cameraPrefab);
                    cvCam = tmp.GetComponent<CinemachineVirtualCamera>();
                }
                cvCam.Follow = transform;
                cvCam.LookAt = transform;
                rb = GetComponent<Rigidbody>();
                toggleCameraLookAt();

#if UNITY_ANDROID
                mobileAxis = FindObjectOfType<FixedJoystick>();
                mobileRT = mobileAxis.shootRegion;
                mobileAxisRT = mobileAxis.transform.parent.GetComponentInParent<RectTransform>();
#endif



            }

            //Instantiate Healthbar
            canvasTransform = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Transform>();
            healthSlider = Instantiate(healthBarPrefab, canvasTransform).GetComponent<Slider>();

            //set current health to max health
            currentHealth = maxHealth;

        }

        private void Update()
        {
            if (photonView.isMine)
            {
                ProcessInput();
                checkHealth();
            }

            
        }

        private void FixedUpdate()
        {
            if (photonView.isMine)
            {
                ProcessMovement();
                ProcessCameraMovement();
            }
            //need to make this part of the photon view eventually
            //this updates the visual healthbar
            healthSlider.value = currentHealth / maxHealth;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.other.gameObject.tag == "Projectile")
            {
                currentHealth -= 25;
            }
        }

#endregion

#region Public Methods

#region Acessors

        public int getTeam() { return team; }

        public float getProjSpeed() { return projSpeed; }

#endregion

#endregion

#region Custom

        /// <summary>
        /// runs at update and updates player input
        /// </summary>
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
                        distance = Mathf.Clamp(distance, transposer.m_XAxis.m_MinValue, transposer.m_XAxis.m_MaxValue);
                        //Debug.Log("MoveCamera: " + distance);
                        
                        float oldValue = transposer.m_XAxis.Value;
                        transposer.m_XAxis.Value = oldValue + (distance / transposer.m_XAxis.m_MaxValue) * mobileLookSpeed;
                        //Input.simulateMouseWithTouches = true;// cvCam.GetInputAxisProvider();
                    }
                }
            }
#endif

            //rotHorizontal = Input.GetAxisRaw("Mouse X");

            


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
            rb.AddForce(transform.localToWorldMatrix * (direction * speed * Time.fixedDeltaTime));

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

        void resetShot()
        {
            projSpeed = 10000;
        }

        void chargeShot()
        {
            projSpeed = (projSpeed + 150);
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
                    proj = PhotonNetwork.Instantiate("testProjectile", boatPosition, boatRotation, 0);
                }

                if (proj == null)
                {
                    return;
                }

                proj.transform.position = new Vector3(boatPosition.x + angleModifier * numProjectiles, boatPosition.y + 4f, boatPosition.z);
                proj.transform.rotation = boatRotation;

                Vector3 front = gameObject.transform.right;
                Vector3 cameraDirection = (Camera.main.transform.position - gameObject.transform.position).normalized;
                Vector3 projectileDirection = new Vector3(-cameraDirection.x, cameraDirection.y, -cameraDirection.z).normalized;
                
                proj.gameObject.GetComponent<Rigidbody>().AddForce(projectileDirection * (projSpeed + speed) * Time.fixedDeltaTime);
            }
        }

        private void ProcessDeath()
        {
            respawn();
            currentHealth = maxHealth;
        }

        void setSpawnPoint()
        {
            Debug.Log("Setting spawn " + photonView.owner);
            spawnPoint = PlayerSpawnManager.self.getSpawnPoint();
            respawn();
        }

        void respawn()
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

        void checkHealth()
        {
            if (currentHealth <= 0)
            {
                photonView.RPC("onDeath", PhotonTargets.All);
            }
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

            GameManager.self.StartCountDown();
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
        void onDeath()
        {
            ProcessDeath();
        }

        [PunRPC]
        void EndState(int winningTeam)
        {
              GameManager.self.loadScene(winningTeam);
        }

#endregion
    }
}
