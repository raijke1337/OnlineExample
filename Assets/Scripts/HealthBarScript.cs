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
using System.Threading.Tasks;

public class HealthBarScript : MonoBehaviour
{

    private float _max;
    private float _current;
    private Image _bar;

    private RectTransform _rect;

    private PlayerController _ch;

    public bool IsActive { get; set; }

    private void FixedUpdate()
    {
        if (!IsActive) return;

        _current = _ch._health;
        _bar.fillAmount = _current / _max;
        _rect.LookAt(Camera.main.transform);

    }


    public void SetUp(PlayerController ctrl)
    {
        _ch = ctrl;
        _current = _max = ctrl._health;
        _bar = GetComponentsInChildren<Image>().First(t => t.gameObject.tag == "Fill");

        IsActive = true;
    }

    private void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

}
