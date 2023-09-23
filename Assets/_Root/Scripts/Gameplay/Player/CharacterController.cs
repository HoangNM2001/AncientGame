using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterController : GameComponent
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private CharacterActionList characterActionList;
    
    private NavmeshController navmeshController;
    private CharacterHandleInput characterHandleInput;

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
    }
    
    protected override void OnDisabled()
    {
        getCharacterEvent.OnRaised -= getCharacterEvent_OnRaised;
    }

    protected override void Tick()
    {
        characterHandleInput.GetInput();
        MoveByDirection(characterHandleInput.MoveDir.normalized, characterStat.moveSpeed, Time.deltaTime);
    }

    public void MoveByDirection(Vector3 direction, float moveSpeed, float deltaTime)
    {
        navmeshController.MoveByDirection(direction, moveSpeed, characterStat.rotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, deltaTime);
    }

    private GameObject getCharacterEvent_OnRaised()
    {
        return gameObject;
    }
}