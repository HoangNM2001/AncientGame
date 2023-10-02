using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterHandleInput : GameComponent
{
    [SerializeField] private Vector2Variable moveDirection;

    private Camera mainCamera;
    private Vector3 cameraForward;

    public Vector3 MoveDir { get; private set; }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void GetInput()
    {   
        MoveDir = moveDirection.Value;

        if (MoveDir == Vector3.zero) return;
        cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        MoveDir = MoveDir.x * mainCamera.transform.right + MoveDir.y * cameraForward;
    }
}
