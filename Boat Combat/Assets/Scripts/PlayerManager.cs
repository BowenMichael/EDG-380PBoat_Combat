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


        private float horizontal;
        private float vertical;
        private int playerLookAtIndex = 0; //The index of the player being looked at by the local client


        private float rotHorizontal;

        [SerializeField] private Image healthBar;
        [SerializeField] private float maxHealth = 100f;
        private float currentHealth;

        private Transform spawnPoint;

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
            }

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
            healthBar.fillAmount = currentHealth / maxHealth;
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
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            //rotHorizontal = -Input.GetAxisRaw("Mouse X");

            //fire projectile
            if(Input.GetKeyDown(KeyCode.Space))
            {
                fireProjectile();
            }

            
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
            Vector3 rotation = new Vector3(0f, rotHorizontal, 0f);
            cvCam.transform.RotateAround(gameObject.transform.position, -Vector3.up, sensitivity * rotHorizontal);
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
            GameObject proj = PhotonNetwork.Instantiate("testProjectile", boatPosition, boatRotation, 0);
            
            proj.transform.position = new Vector3(boatPosition.x, boatPosition.y + 1f, boatPosition.z);
            //proj.transform.rotation = new Quaternion(boatRotation.x, boatRotation.y, boatRotation.z + 100f, boatRotation.w);
            //proj.transform.rotation.Set += 90f;
            proj.transform.rotation = boatRotation;
            Vector3 front = gameObject.transform.right;
            Vector3 cameraDirection = (Camera.main.transform.position - gameObject.transform.position).normalized;
            Vector3 projectileDirection = new Vector3(-cameraDirection.x, cameraDirection.y, -cameraDirection.z).normalized;
            proj.gameObject.GetComponent<Rigidbody>().AddForce(projectileDirection * projSpeed * Time.fixedDeltaTime);
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

        #endregion
    }
}
