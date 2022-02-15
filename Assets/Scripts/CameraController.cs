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

public class CameraController : MonoBehaviour
{
    public Transform Target { get; set; }
    [SerializeField,Tooltip("Camera offset for player follow")]
    private Vector3 _camOffset = new Vector3(0,10,-10);

    private void FixedUpdate()
    {
        if (Target == null) return;
        transform.position = Target.position + _camOffset;
    }

}
