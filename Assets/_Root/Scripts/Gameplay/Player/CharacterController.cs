using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterController : GameComponent
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private CharacterAnimController characterAnimController;

    private NavmeshController navmeshController;
    private CharacterHandleInput characterHandleInput;
    private EnumPack.ControlType controlType;
    private float currentMoveSpeed;

    public CharacterStat CharacterStat => characterStat;
    public CharacterAnimController CharacterAnimController => characterAnimController;

    private void Awake()
    {
        navmeshController = GetComponent<NavmeshController>();
        characterHandleInput = GetComponent<CharacterHandleInput>();
    }

    protected override void OnEnabled()
    {
        getCharacterEvent.OnRaised += getCharacterEvent_OnRaised;
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
    }

    private GameObject getCharacterEvent_OnRaised()
    {
        return gameObject;
    }

    private void changeInputEvent_OnRaised(int newControlType)
    {
        controlType = (EnumPack.ControlType)newControlType;
        navmeshController.ResetPath();
    }

    protected override void OnDisabled()
    {
        getCharacterEvent.OnRaised -= getCharacterEvent_OnRaised;
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
    }

    private void Start()
    {
        currentMoveSpeed = characterStat.moveSpeed;
        controlType = EnumPack.ControlType.Move;
    }

    protected override void Tick()
    {
        if (controlType != EnumPack.ControlType.Move) return;
        
        characterHandleInput.GetInput();
        MoveByDirection(characterHandleInput.MoveDir.normalized, currentMoveSpeed, Time.deltaTime);
    }

    private void MoveByDirection(Vector3 direction, float moveSpeed, float deltaTime)
    {
        navmeshController.MoveByDirection(direction, moveSpeed, characterStat.rotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, deltaTime);
    }

    public void MoveToPosition(Vector3 position, float moveSpeed, float deltaTime)
    {
        navmeshController.MoveByPosition(position, 0.0f, moveSpeed, characterStat.rotateSpeed, 0.1f, deltaTime);
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, deltaTime);
    }

    public void RotateToTarget(Vector3 pos, float deltaTime)
    {
        var dir = pos - transform.position;
        dir.y = 0.0f;
        navmeshController.MoveByDirection(dir, 0, characterStat.rotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, deltaTime);
    }

    public void ChangeToWorkingMoveSpeed()
    {
        currentMoveSpeed = characterStat.workingMoveSpeed;
    }

    public void ResetBackToMoveSpeed()
    {
        currentMoveSpeed = characterStat.moveSpeed;
    }
}