using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CameraVariableSetter : GameComponent
{
    [SerializeField] private CameraVariable cameraVariable;

    private void Awake()
    {
        cameraVariable.Value = GetComponent<Camera>();
    }
}
