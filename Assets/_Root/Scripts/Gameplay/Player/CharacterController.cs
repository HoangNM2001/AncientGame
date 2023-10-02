using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterController : GameComponent
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventVector3 moveToTreeEvent;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private CharacterAnimController characterAnimController;
    
    private NavmeshController navmeshController;
    private CharacterHandleInput characterHandleInput;
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
        // moveToTreeEvent.OnRaised += moveToTreeEvent_OnRaised;
    }

    private void moveToTreeEvent_OnRaised(Vector3 movePosition)
    {
        MoveToPosition(movePosition, characterStat.workingMoveSpeed, Time.deltaTime);
    }

    private GameObject getCharacterEvent_OnRaised()
    {
        return gameObject;
    }
    
    protected override void OnDisabled()
    {
        getCharacterEvent.OnRaised -= getCharacterEvent_OnRaised;
        // moveToTreeEvent.OnRaised -= moveToTreeEvent_OnRaised;
    }

    private void Start() 
    {
        currentMoveSpeed = characterStat.moveSpeed;    
    }

    protected override void Tick()
    {
        characterHandleInput.GetInput();
        MoveByDirection(characterHandleInput.MoveDir.normalized, currentMoveSpeed, Time.deltaTime);
    }

    private void MoveByDirection(Vector3 direction, float moveSpeed, float deltaTime)
    {
        navmeshController.MoveByDirection(direction, moveSpeed, characterStat.rotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, deltaTime);
    }

    private void MoveToPosition(Vector3 position, float moveSpeed, float deltaTime)
    {
        navmeshController.MoveByPosition(position, 0, moveSpeed, characterStat.rotateSpeed, 0.1f, deltaTime);
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