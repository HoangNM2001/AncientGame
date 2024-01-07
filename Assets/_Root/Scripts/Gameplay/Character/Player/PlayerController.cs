using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerController : GameComponent
{
    [SerializeField] private IntVariable goldVariable;
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private PlayerActionList playerActionList;
    [SerializeField] private CharacterHandleTrigger characterHandleTrigger;
    [SerializeField] private Vector3Variable playerPosition;
    [SerializeField] private Vector3Variable playerRotation;
    [SerializeField] private GameObject buildHammer;
    [SerializeField] private GameObject buildFx;

    private NavmeshController navmeshController;
    private PlayerHandleInput playerHandleInput;
    private EnumPack.ControlType controlType;
    private float currentMoveSpeed;
    private bool isBuilding;
    private bool moveByBike;
    private Tile currentTile;

    public CharacterStat CharacterStat => characterStat;
    public CharacterAnimController CharacterAnimController => characterAnimController;
    private bool CanBuild => playerActionList.CurrentCharacterAction == null;

    private void Awake()
    {
        navmeshController = GetComponent<NavmeshController>();
        playerHandleInput = GetComponent<PlayerHandleInput>();

        // transform.position = playerPosition.Value;
        // transform.rotation = Quaternion.Euler(playerRotation.Value);
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
        if (isBuilding) return;
        if (controlType != EnumPack.ControlType.Move) return;
        
        playerHandleInput.GetInput();
        MoveByDirection(playerHandleInput.MoveDir.normalized, currentMoveSpeed, Time.deltaTime);

        // playerPosition.Value = transform.position;
        // playerRotation.Value = transform.rotation.eulerAngles;
    }

    public void CheckToBuild()
    {
        if (CanBuild && characterHandleTrigger.CurrentInteract.TryGetComponent<Tile>(out var tile))
        {
            if (goldVariable.Value > tile.UnlockCost)
            {
                currentTile = tile;
                Build();
            }
        }
    }

    public void Build()
    {
        isBuilding = true;
        goldVariable.Value -= currentTile.UnlockCost;
        buildHammer.SetActive(true);
        buildFx.transform.position = currentTile.transform.position;
        buildFx.SetActive(true);
        characterAnimController.Play(Constant.BUIDLING, 0);

        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(StopBuild);
    }

    public void StopBuild()
    {
        isBuilding = false;
        currentTile.Unlock();
        buildHammer.SetActive(false);
        buildFx.SetActive(false);
        BackToMove();
    }

    private void BackToMove()
    {
        characterAnimController.Play(moveByBike ? Constant.IDLE_2_BIKE : Constant.IDLE_2_RUN, 0);
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