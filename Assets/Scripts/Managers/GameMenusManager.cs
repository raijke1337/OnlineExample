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
using Photon.Realtime;

public class GameMenusManager : MonoBehaviourPunCallbacks
{
    private Text _info;
    private Button _butt;

    [SerializeField, Range(1, 3), Tooltip("Delay till button is active")]
    private float _delay = 2f;


    public void ToMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    private void Awake()
    {
        Debugger.OnStart();
        Debugger.PrintLog("Loaded");
    }

    public void Show(string winner)
    {
        _info = GetComponentInChildren<Text>();
        _butt = GetComponentInChildren<Button>();

        _info.text = "Winner is: " + winner;
        StartCoroutine(ActivateButtonCor());
    }
    private IEnumerator ActivateButtonCor()
    {
        yield return new WaitForSeconds(_delay);
        _butt.interactable = true;
    }

    public override void OnLeftRoom()
    {
        Debugger.PrintLog("You left the room");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debugger.PrintLog($"Player {otherPlayer.NickName} has left the room");
    }



}
