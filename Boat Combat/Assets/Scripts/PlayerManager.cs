using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon;
using Photon.Realtime;

namespace Com.BowenIvanov.BoatCombat
{
    public class PlayerManager : PunBehaviour//, IPunObservable//(for sending specific data)
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

        private float horizontal;
        private float vertical;
        private int team;

        private float rotHorizontal;

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
                init();
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
                rb = GetComponent<Rigidbody>();
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

        #endregion

        #region Custom

        /// <summary>
        /// runs at update and updates player input
        /// </summary>
        void ProcessInput()
        {
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            rotHorizontal = -Input.GetAxisRaw("Mouse X");

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

        void fireProjectile()
        {
            Vector3 boatPosition = gameObject.transform.position;
            Quaternion boatRotation = gameObject.transform.rotation;
            GameObject proj = GameObject.Instantiate(testProjectile);
            
            proj.transform.position = new Vector3(boatPosition.x, boatPosition.y + 1f, boatPosition.z);
            //proj.transform.rotation = new Quaternion(boatRotation.x, boatRotation.y, boatRotation.z + 100f, boatRotation.w);
            //proj.transform.rotation.Set += 90f;
            proj.transform.rotation = boatRotation;
            Vector3 front = gameObject.transform.right;
            proj.gameObject.GetComponent<Rigidbody>().AddForce(front * 100000 * Time.fixedDeltaTime);
        }

        void setSpawnPoint()
        {
            Debug.Log("Setting spawn " + photonView.owner);
            Transform newTransform = PlayerSpawnManager.self.getSpawnPoint();
            photonView.RPC("sendSpawnPoint", PhotonTargets.All, new float[] { newTransform.position.x, newTransform.position.y, newTransform.position.z },
                                                                new float[] { newTransform.rotation.eulerAngles.x, newTransform.rotation.eulerAngles.y, newTransform.rotation.eulerAngles.z });
        }

        void setTeamByPlayerManager()
        {
            team = PlayerSpawnManager.self.getTeam();
            photonView.RPC("sendTeam", PhotonTargets.AllBuffered, team);
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

        #endregion
    }
}
