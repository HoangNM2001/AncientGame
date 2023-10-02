using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterHandleInput : GameComponent
{
    [SerializeField] private Vector2Variable moveDirection;
    [SerializeField] private ScriptableEventInt changeInputEvent;

    private Camera mainCamera;
    private Vector3 cameraForward;
    private EnumPack.ControlType controlType;

    public Vector3 MoveDir { get; private set; }
    
    protected override void OnEnabled()
    {
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
    }

    private void changeInputEvent_OnRaised(int newControlType)
    {
        controlType = (EnumPack.ControlType)newControlType;
    }

    protected override void OnDisabled()
    {
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        controlType = EnumPack.ControlType.Move;
    }

    public void GetInput()
    {
        if (controlType != EnumPack.ControlType.Move) return;
        
        MoveDir = moveDirection.Value;

        if (MoveDir == Vector3.zero) return;
        cameraForward = mainCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        MoveDir = MoveDir.x * mainCamera.transform.right + MoveDir.y * cameraForward;
    }
}
