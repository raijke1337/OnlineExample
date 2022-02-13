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
    private void Start()
    {
        // required params for networking

        // usually externally saved obviously
        // must be unique
#if UNITY_EDITOR
        PhotonNetwork.NickName = "pwnz";
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        PhotonNetwork.NickName = "n00b";
#endif
        // automatically sync scene change when all clients join
        PhotonNetwork.AutomaticallySyncScene = true;
        //set version of build, must match for all players
        PhotonNetwork.GameVersion = "0.0.1";

    }


    public void OnCreateRoom()
    {

    }
    public void OnJoinRoom()
    {

    }



}
