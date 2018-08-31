using UnityEngine;

namespace Com.Soho.MyGame
{
    public class PlayerManager : Photon.PunBehaviour, IPunObservable
    {
        #region Public Variables

        public static GameObject LocalPlayerInstance;

        [Tooltip("The Beams GameObject to control")]
        public GameObject Beams;

        [Tooltip("The current Health of our player")]
        public float Health = 1f;

        #endregion

        #region Private Variables

        bool isFiring;

        #endregion

        #region PunBehaviour CallBacks

        private void Awake()
        {
            if (photonView.isMine)
            {
                LocalPlayerInstance = this.gameObject;
            }

            DontDestroyOnLoad(this.gameObject);

            if (Beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                Beams.SetActive(false);
            }
        }

        private void Start()
        {
            CameraWork _cameraWork = this.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.isMine)
                {
                    _cameraWork.OnStartFollowing();
                }
                else
                {
                    Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);
                }
            }

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
            {
                this.CallOnLevelWasLoaded(scene.buildIndex);
            };
#endif
        }

        private void Update()
        {
            if (photonView.isMine)
            {
                ProcessInputs();
            }

            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
            if (Beams != null && isFiring != Beams.GetActive())
            {
                Beams.SetActive(isFiring);
            }

        }

#if UNITY_5_4_OR_NEWER
        /// <summary>
        /// See CalledOnLevelWasLoaded. Outdated in Unity 5.4
        /// </summary>
        /// <param name="level">Level.</param>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

        void CalledOnLevelWasLoaded(int level)
        {
            // Check if we are outside of Arena and if it's the case,spawn around 
            // the center of the arena in a safe zone
            if(!Physics.Raycast(transform.position,-Vector3.up,5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if(!photonView.isMine)
            {
                return;
            }

            if(!other.name.Contains("Beam"))
            {
                return;
            }

            Health -= 0.1f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.isMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f * Time.deltaTime;
        }
#endregion

#region Custom

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (!isFiring)
                {
                    isFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (isFiring)
                {
                    isFiring = false;
                }
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.isWriting)
            {
                stream.SendNext(isFiring);
                stream.SendNext(Health);
            }
            else
            {
                this.isFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }

        }

#endregion
    }
}
