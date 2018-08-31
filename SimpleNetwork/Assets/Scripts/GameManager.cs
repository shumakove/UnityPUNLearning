using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Com.Soho.MyGame  
{
    public class GameManager : Photon.PunBehaviour
    {
        #region Public properties

        static public GameManager Instance;
        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;

        #endregion 

        #region Photon Messages

        /// <summary>
        /// Called when the local player left the room. We need to load th launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        private void Start()
        {
            Instance = this;
            if(playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference." +
                               " Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                Debug.Log("We are instantiating localPlayer from " + Application.loadedLevelName);
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            }

        }

        #endregion

        #region Public Methods
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region Private Methods
        void LoadArena()
        {
            if (!PhotonNetwork.isMasterClient)
            {
                Debug.LogError("PhotonNetwork: Trying to Load a level but we are not" +
                               "master Client");
            }
            Debug.Log("PhotonNetwork: Loading level " + PhotonNetwork.room.playerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
        }
        #endregion

        #region Photon Messages

        public override void OnPhotonPlayerConnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerConnected() " + other.NickName);

            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
            }

            LoadArena();
            Debug.Log(PhotonNetwork.room.PlayerCount);
        }

        public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
        {
            Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName); // seen when other disconnects


            if (PhotonNetwork.isMasterClient)
            {
                Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient); // called before OnPhotonPlayerDisconnected


                LoadArena();
            }
        }

        #endregion
    }
}