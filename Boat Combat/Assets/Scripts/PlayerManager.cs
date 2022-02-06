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

        #endregion

        #region Private Variables

        [SerializeField]
        private CinemachineVirtualCamera cvCam;

        private Rigidbody rb;

        [SerializeField] private float speed;
        [SerializeField] private float rotSpeed;

        private float horizontal;
        private float vertical;

        #endregion

        #region Photon Callbacks

        public override void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            info.sender.TagObject = this.gameObject;
        }

        #endregion

        #region RPC

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

                photonView.RPC("setSpawnPoint", PhotonTargets.MasterClient);
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
            rb.AddTorque(rotation * rotSpeed * Time.fixedDeltaTime);
        }

        /// <summary>
        /// RPC Function that sets the spawn position. Only to run on the master client
        /// </summary>
        [PunRPC]
        void setSpawnPoint()
        {
            Debug.Log("Setting spawn " + photonView.owner);
            Transform newTransform = PlayerSpawnManager.self.getSpawnPoint();
            transform.position = newTransform.position; 
            transform.rotation = newTransform.rotation; 
        }

        #endregion
    }
}
