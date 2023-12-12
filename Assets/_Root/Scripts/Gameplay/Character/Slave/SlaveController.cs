using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEditor.AddressableAssets.Build.BuildPipelineTasks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class SlaveController : GameComponent, IFarmer, ICaveMan
{
    [SerializeField] private ExtendField extendField;
    [SerializeField] private ResourcesCave cave;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private CharacterActionList characterActionList;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private Transform staffBackpack;
    [SerializeField] private ScriptableEventStorageAddData addStorageEvent;

    private NavmeshController navmeshController;
    private StateMachine stateMachine;
    private Vector3 targetPostion;
    private EnumPack.FieldState currentFarmState;

    // Test Variables
    private ResourceConfig currentResource;
    public int currentCapacity = 0;
    public int maxCapacity = 10;
    public bool IsRelaxing = true;

    public CharacterActionList ActionList => characterActionList;
    private bool IsBackpackFull => currentCapacity == maxCapacity;

    private void Awake()
    {
        navmeshController = GetComponent<NavmeshController>();
        
        stateMachine = new StateMachine();
        stateMachine.InitStates(new RelaxState(this), new MovingState(this), new FarmingState(this));
        stateMachine.ChangeState<RelaxState>();
    }

    protected override void Tick()
    {
        stateMachine.Tick();
    }

    public void Relaxing()
    {
        if (cave.IsCaveFull())
        {
            navmeshController.Stop();

            if (characterAnimController.AnimationName != Constant.SLAVE_RELAX)
            {
                characterAnimController.Play(Constant.SLAVE_RELAX, 1);
            }
        }
        else
        {
            if (IsBackpackFull)
            {
                GoToCave();
            }
            else
            {
                GoToField();
            }
        }
    }

    public void Moving()
    {
        characterAnimController.UpdateIdle2Run(navmeshController.VelocityRatio, Time.deltaTime);
        navmeshController.MoveByPosition(targetPostion, 0.0f, characterStat.moveSpeed, characterStat.rotateSpeed, 0.1f, Time.deltaTime);
    }

    public void Farming()
    {
        if (navmeshController.IsReachDestination())
        {
            GoToNearestField();
        }
    }

    public void GoToNearestField()
    {
        var nearestField = extendField.GetNearestFieldWithState(transform.position + transform.forward * 20.0f, currentFarmState);
        if (nearestField)
        {
            navmeshController.MoveByPosition(nearestField.transform.position, 1.0f, characterStat.workingMoveSpeed, characterStat.rotateSpeed, 0.1f, Time.deltaTime);
        }
    }

    private void OnFieldStateChange()
    {
        ActiveFarmAction();
    }

    private void OnHarvest(bool isHarvestDone)
    {
        currentCapacity++;

        if (IsBackpackFull || isHarvestDone)
        {
            GoToCave();
        }
    }

    public void TriggerActionFarm(GameObject gameObject = null)
    {
        extendField.OnStateChange += OnFieldStateChange;
        extendField.OnHarvest += OnHarvest;
        stateMachine.ChangeState<FarmingState>();
    }

    public void ExitTriggerActionFarm()
    {
        extendField.OnStateChange -= OnFieldStateChange;
        extendField.OnHarvest -= OnHarvest;
        characterActionList.StopActionEvent();
    }

    public void TriggerActionCave(GameObject gameObject = null)
    {
        var tempCapacity = currentCapacity;
        currentCapacity = cave.AddStorage(currentResource.resourceType, tempCapacity, () => DOTween.Sequence().AppendInterval(2.0f).AppendCallback(GoToField));
    }

    public void ExitTriggerActionCave()
    {
        
    }

    public void ActiveFarmAction()
    {
        currentFarmState = extendField.TransferFarmState;

        switch (currentFarmState)
        {
            case EnumPack.FieldState.Seedale:
                ActiveSeed();
                break;
            case EnumPack.FieldState.Waterable:
                ActiveWater();
                break;
            case EnumPack.FieldState.Harvestable:
                ActiveHarvest();
                break;
            default:
                break;
        }
    }

    private void ActiveSeed()
    {
        if (!extendField.IsSeeded()) extendField.InitializeOnNewSeed(cave.SuitableResouce());

        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.SeedFarm);
    }

    private void ActiveWater()
    {
        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.WaterFarm);
    }

    private void ActiveHarvest()
    {
        currentResource = extendField.ResourceConfig;
        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.HarvestFarm);
    }

    private void GoToField()
    {
        targetPostion = extendField.transform.position;
        stateMachine.ChangeState<MovingState>();
    }

    private void GoToCave()
    {
        targetPostion = cave.GoToPos.position;
        stateMachine.ChangeState<MovingState>();
    }

    private bool IsCaveFull()
    {
        return cave.IsCaveFull();
    }

    public void EmptyLayer1()
    {
        characterAnimController.Play(Constant.EMPTY, 1);
    }
}
