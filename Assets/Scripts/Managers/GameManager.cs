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
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private InputAction _quit;

    private void Start()
    {
        _quit.performed += _quit_performed;
        _quit.canceled += DebugMessage;
    }

    private void _quit_performed(InputAction.CallbackContext obj)
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
        Application.Quit();
#endif
    }
    private void DebugMessage(InputAction.CallbackContext obj)
    {
        Debug.Log("Hold ESC for 2 seconds to quit the game");
    }

}
