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

public class ParticlesCOntroller : MonoBehaviour
{
    private ParticleSystem _muzzleFlash;
    private ParticleSystem _deathEffect;


    private void Start()
    {
        _muzzleFlash = GetComponentsInChildren<ParticleSystem>().First(t => t.name == "MuzzleFlash");
    }

    public void MuzzleFlash()
    {
        _muzzleFlash.Play();
    }


}
