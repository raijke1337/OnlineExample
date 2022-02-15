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
using UnityEngine.InputSystem;
using Photon.Pun;
using ExitGames.Client.Photon;

public class GameManager : MonoBehaviourPunCallbacks
{
    #region singleton

    public static GameManager instance;
    private void Awake()
    {
        if (instance != null) Destroy(instance);
        instance = this;
    }

    #endregion



    [SerializeField]
    private InputAction _quit;
    [SerializeField]
    private string _playerPrefabName;

    private PlayerController _p1;
    private PlayerController _p2;

       
    [SerializeField]
    private List<Transform> _spawns;

    public Transform BulletsPool { get => _bulletsPool; set => _bulletsPool = value; }

    private Transform _bulletsPool;
    // used to clear bullets on session over

    private GameMenusManager _panel;

    private void Start()
    {
        _quit.Enable();
        _quit.performed += _quit_performed;
        _quit.canceled += DebugMessage;

        _spawns = new List<Transform>();
        _spawns.AddRange(GetComponentsInChildren<Transform>().Where(t => t.name.Contains("Spawn")));
        _spawns.TrimExcess();

        BulletsPool = GameObject.Find("BulletsPool").transform;

        _panel = GameObject.Find("GameResults").GetComponent<GameMenusManager>();
        _panel.gameObject.SetActive(false);

        #region netcode
        // the name of player prefabs
        // position is set in "addplayer"
        var GO = PhotonNetwork.Instantiate(_playerPrefabName + PhotonNetwork.NickName, Vector3.zero, new Quaternion());
        // register the serializer
        PhotonPeer.RegisterType(typeof(PlayerData), 1, 
            CodeExtensions.SerializePlayerData, CodeExtensions.DeserializePlayerData);
        
        
        #endregion
    }

    private void _quit_performed(InputAction.CallbackContext obj)
    {
        PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Application.Quit();
#endif
    }
    private void DebugMessage(InputAction.CallbackContext obj)
    {
        Debugger.PrintLog("Hold ESC for 2 seconds to quit the game");
    }

    #region netcode
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        SceneManager.LoadScene(0);
        // todo score scene
    }
    #endregion

    public void AddPlayer(PlayerController player)
    {
        player.ZeroHealthEvent += ZeroHealthEventHandler;
        //var used = _spawns[Random.Range(0, _spawns.Capacity-1)];
        //player.transform.position = used.position;
        //_spawns.Remove(used);
        //_spawns.TrimExcess();
        //places player on random spawner and removes it from list
        // doesnt work in net mode - spawns can be the same

        if (player.name.Contains("1"))
        {
            _p1 = player;
            player.transform.position = _spawns[0].position;
        }
        else
        {
            _p2 = player;
            player.transform.position = _spawns[2].position;
            // bandaid todo
        }

        // this runs on joining player
        if (_p1 != null && _p2 != null)
        {
            _p1.SetTarget(_p2.transform);
            _p2.SetTarget(_p1.transform);
        }
    }

    private void ZeroHealthEventHandler(object sender, PlayerController e)
    {
        _p1.SessionOver();
        if (_p2!=null) _p2.SessionOver();
        _panel.gameObject.SetActive(true);

        Debugger.PrintLog($"{e.name} was killed!");
        var txt = FindObjectsOfType<PlayerController>().First(t => t._health > 0f).gameObject.name;

        _panel.Show(txt);
        
    }
    private void OnDestroy()
    {
        _quit.Dispose();
    }

    #region UNITY_EDITOR
#if UNITY_EDITOR
    public void QuickTestStart()
    {
        // for debug purposes
        _p1.SetTarget(transform);
    }

#endif
#endregion


}
