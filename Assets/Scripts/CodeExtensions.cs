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

public static class Debugger
{
    private static Text _console;

    // run method after the main menu scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void OnStart()
    {
        _console = GameObject.FindObjectsOfType<Text>().First(t => t.name == "Console");
        PrintLog("Console found, object: " + _console.gameObject);
    }

    public static void PrintLog(object msg)
    {
#if UNITY_EDITOR
        Debug.Log(msg);
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR        
        _console.text += msg + "\n";
#endif
    }
}

#region netcode

public class CodeExtensions
{
    // serializer for photon
    // implemented in Start() GAMEMANAGER
    public static byte[] SerializePlayerData(object data)
    {
        var player = (PlayerData)data;
        var array = new List<Byte>(16);
        // 4 floats * 4 bytes = 16 bytes

        array.AddRange(BitConverter.GetBytes(player.posX));
        array.AddRange(BitConverter.GetBytes(player.posZ));
        array.AddRange(BitConverter.GetBytes(player.rotY));
        array.AddRange(BitConverter.GetBytes(player.hp));

        return array.ToArray();

    }
    public static object DeserializePlayerData(byte[] data)
    {
        return new PlayerData
        {
            posX = BitConverter.ToSingle(data, 0),
            posZ = BitConverter.ToSingle(data, 4),
            rotY = BitConverter.ToSingle(data, 8),
            hp = BitConverter.ToSingle(data, 12)
        };
    }
}

// data to be synced
public struct PlayerData
{
    public float posX;
    public float posZ;
    public float rotY;
    public float hp;

    public static PlayerData Create(PlayerController player)
    {
        return new PlayerData
        {
            posX = player.transform.position.x,
            posZ = player.transform.position.z,
            rotY = player.transform.eulerAngles.y,
            hp = player._health
        };
    }
    public void Set(PlayerController player)
    {
        var vector = player.transform.position;
        vector.x = posX;
        vector.z = posZ;
        player.transform.position = vector;

        vector = player.transform.eulerAngles;
        vector.y = rotY;
        player.transform.eulerAngles = vector;

        player._health = hp;
    }


}


#endregion