using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;


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
        [SerializeField] private float fireRate;
        [SerializeField] private int maxAmmo;

        public GameObject testProjectile;
        public GameObject reloadText;
        public GameObject weaponBarrel;

        #endregion

        #region Private Variables


        private float horizontal;
        private float vertical;
        private float lastFired;
        private int currentAmmo;
        private bool isReloading = false;

        PlayerManager plm;
        RectTransform mobileFireRT;

        #endregion

        #region MonoBehavior Callbacks
        // Start is called before the first frame update
        void Awake()
        {
            //currentProjectile = "testProjectile";//default

        }

        private void Start()
        {
            plm = GetComponent<PlayerManager>();
            mobileFireRT = plm.getFireRT();

            lastFired = Time.time;
            currentAmmo = maxAmmo;

            reloadText = GameObject.Find("/Canvas/MainUI/Reload Text");
            //reloadText = temp.GetComponent<Canvas>();

            mobileFireRT = FindObjectOfType<MobileManager>().getShoot();
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
#if !(UNITY_ANDROID || UNITY_IOS)
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");

            //fire projectile
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
            {
                resetShot();
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0))
            {
                if (isReloading)
                    return;

                if(Time.time - lastFired > 1 / fireRate)
                {
                    lastFired = Time.time;
                    fireProjectile();
                    currentAmmo--;
                    if(currentAmmo <= 0)
                    {
                        StartCoroutine(reloadWeapon());
                    }
                }
                //chargeShot();
            }

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
            {
                //fireProjectile();
            }

            if(Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
            {
                StartCoroutine(reloadWeapon());
            }

#else
            //Camera Controls
            Touch[] ts = Input.touches;


            for (int i = 0; i < ts.Length; i++)
            {
                if ((ts[i].position.x < mobileFireRT.position.x + mobileFireRT.rect.width * .5f && ts[i].position.y < mobileFireRT.position.y + mobileFireRT.rect.height * .5f))
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
                proj.transform.position = weaponBarrel.transform.position;//new Vector3(boatPosition.x , boatPosition.y, boatPosition.z);

                proj.transform.forward = weaponBarrel.transform.right;// new Vector3(-cameraDirection.x, 0f, -cameraDirection.z);

                //proj.transform.position += (proj.transform.forward * 10);



                Vector3 front = gameObject.transform.right;

                //Vector3 projectileDirection = new Vector3(-cameraDirection.x, 0f, -cameraDirection.z).normalized;
                Vector3 projectileDirection = new Vector3 (proj.transform.forward.x, 0f, proj.transform.forward.z);
                proj.transform.rotation = new Quaternion(boatRotation.x + 90f, boatRotation.y -90f, boatRotation.z, boatRotation.w);
                proj.gameObject.GetComponent<Rigidbody>().AddForce(projectileDirection * (projSpeed + speed + 1000) * Time.fixedDeltaTime);
            }
        }

        IEnumerator reloadWeapon()
        {
            isReloading = true;
            Debug.Log("Reloading");

            //reload text shows player they are reloading
            reloadText.SetActive(true);

            //this needs to be tested
            yield return new WaitForSeconds(1.5f);
            currentAmmo = maxAmmo;

            reloadText.SetActive(false);
            isReloading = false;
        }

        

        #endregion

        #region Acessors
        public float getProjSpeed() { return projSpeed; }

        #endregion

    }




}
