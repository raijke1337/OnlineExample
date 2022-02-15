using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class MenusManager : MonoBehaviourPunCallbacks
{
    // note the parent class

    public void OnQuit_UE()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Application.Quit();
#endif
    }
    #region netcode
    private void Start()
    {
        // required params for networking

        // usually externally saved obviously
        // must be unique
#if UNITY_EDITOR
        PhotonNetwork.NickName = "1";
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        PhotonNetwork.NickName = "2";
#endif
        // automatically sync scene change when all clients join
        PhotonNetwork.AutomaticallySyncScene = true;
        //set version of build, must match for all players
        PhotonNetwork.GameVersion = "0.0.1";
        // connect to server using default settings (set when installing)
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // empty
        // when connected to lobby creator master server
        Debugger.PrintLog("Ready to connect to room");
    }

    public override void OnJoinedRoom()
    {
        // also empty
        // when joined lobby
        Debugger.PrintLog("Joined game room");
        // load like this to sync scenes
        PhotonNetwork.LoadLevel("NetGameScene");
    }

    public void OnCreateRoom()
    {
        // create a room
        // research room options
        PhotonNetwork.CreateRoom(null,new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        Debugger.PrintLog("Created a room, waiting for connection");
    }
    public void OnJoinRoom()
    {
        // obvious
        PhotonNetwork.JoinRandomRoom();
    }
    #endregion


}
