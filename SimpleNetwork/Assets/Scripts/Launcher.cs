﻿using UnityEngine;
using UnityEngine.UI;

namespace Com.Soho.MyGame 
{
    public class Launcher : Photon.PunBehaviour
    {
        #region Public Variables

        /// <summary>
        /// The PUN loglevel
        /// </summary>

        public PhotonLogLevel loglevel = PhotonLogLevel.Informational;

        /// <summary>
        /// The maximum number of players. When a room is full, it can't be joined by new players and
        /// so new room will be created
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, " +
                 "it can't be joined by new players and so new room will be created")]
        public byte MaxPlayersRoom = 4;

        [Tooltip("The UI Panel to et the user enter name, connect and play")]
        public GameObject controlPanel;
        [Tooltip("The UI Label to inform the user that the connection is in progress")]
        public GameObject progressLabel;

        #endregion

        #region Private Variables

        /// <summary>
        /// This client's version number. Users are separated from each other by
        /// game version (which allows you to make breaking changes).
        /// </summary>

        string _gameVersion = "1";

        /// <summary>
        /// Keep track of current process. Since connection as asynchronous and
        /// is based on several callbacks from Photon,
        /// we need to keep track of this to properly adjust the behaviour when we
        /// receive call back by Photon. 
        /// Typically this is used for the OnConnectedToMaster() callback
        /// </summary>
        bool isConnecting;

        #endregion

        #region MonoBehaviour CallBacks


        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {


            // #Critical
            // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
            PhotonNetwork.autoJoinLobby = false;


            // #Critical
            // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.automaticallySyncScene = true;

            PhotonNetwork.logLevel = loglevel;
        }

        private void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Methods
        ///  <summary>
        /// Start the connection proccess
        /// - If allready conected, we attempt joining a random room
        /// - If not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>

        public void Connect()
        {
            isConnecting = true;

            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            // we check if we are connected or not, we join if we are, else we initiate the connection to the server
            if (PhotonNetwork.connected)
            {
                // #Critical we need at this point to attempt joining Random Room. If it fail's we'll get notified
                // in OnPhotonRandomJoinFailed and we'll create one
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                // #Critical, we must first and foremost connect to Photon Online Server
                PhotonNetwork.ConnectUsingSettings(_gameVersion);
            }
        }

        #endregion

        #region Photon.PunBehaviour CallBacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");
            if (isConnecting)
            {
                PhotonNetwork.JoinRandomRoom();
                progressLabel.GetComponent<Text>().text = "Connected";
            }
        }

        public override void OnDisconnectedFromPhoton()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called ny PUN");
        }

        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("DemoAnimator/Launcher: OnPhotonRandomJoinFailed() was called by PUN. No random room avialable," +
                      "so we create one.\nCalling: Photon.CreateRoom(null,new RoomOptions() {maxPlayers = 4},null);");

            //#Critical: we failed to join a random room, maybe none exists or they are all full.
            PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersRoom }, null);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. now this client is in a room.");

            if(PhotonNetwork.room.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1");

                PhotonNetwork.LoadLevel("Room for 1");
            }
        }

        #endregion
    }
}