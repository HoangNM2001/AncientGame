using System;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using UnityEngine;

public class SlaveController : GameComponent, IFarmer, ICaveMan
{
    [SerializeField] private ExtendField extendField;
    [SerializeField] private ResourcesCave cave;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private CharacterActionList characterActionList;
    [SerializeField] private CharacterStat characterStat;
    [SerializeField] private Transform staffBackpack;

    private NavmeshController navmeshController;
    private StateMachine stateMachine;
    private Vector3 targetPosition;
    private EnumPack.FieldState currentFarmState;
    private SphereCollider sphereCollider;

    // Test Variables
    private EnumPack.ResourceType currentResourceType = EnumPack.ResourceType.None;
    private int currentCapacity;
    private const int MAX_CAPACITY = 10;

    public SphereCollider SphereCollider => sphereCollider;
    public CharacterActionList ActionList => characterActionList;
    private bool IsBackpackFull => currentCapacity >= MAX_CAPACITY;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
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
        if (!cave.IsCaveAvailable || !cave.IsAddable(currentResourceType))
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
        navmeshController.MoveByPosition(targetPosition, 0.0f, characterStat.moveSpeed, characterStat.rotateSpeed, 0.1f,
            Time.deltaTime);
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
        var nearestField =
            extendField.GetNearestFieldWithState(transform.position + transform.forward * 20.0f, currentFarmState);
        if (nearestField)
        {
            navmeshController.MoveByPosition(nearestField.transform.position, 1.0f, characterStat.workingMoveSpeed,
                characterStat.rotateSpeed, 0.1f, Time.deltaTime);
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

    public void TriggerActionCave(GameObject interactCave)
    {
        if (!cave.IsAddable(currentResourceType))
        {
            stateMachine.ChangeState<RelaxState>();
        }
        else
        {
            DOTween.Sequence().AppendInterval(1f).AppendCallback(() =>
            {
                currentCapacity = cave.AddStorage(currentResourceType, currentCapacity, GoToField);
                if (currentCapacity == 0) currentResourceType = EnumPack.ResourceType.None;
            });
        }
    }

    public void ExitTriggerAction()
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
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ActiveSeed()
    {
        if (!extendField.IsSeeded())
        {
            extendField.InitializeOnNewSeed(cave.SuitableResource());
        }
        else
        {
            if (currentResourceType != extendField.ResourceConfig.resourceType &&
                !cave.IsAddable(extendField.ResourceConfig.resourceType))
            {
                stateMachine.ChangeState<RelaxState>();
                return;
            }
        }

        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.SeedFarm);
    }

    private void ActiveWater()
    {
        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.WaterFarm);
    }

    private void ActiveHarvest()
    {
        currentResourceType = extendField.ResourceConfig.resourceType;
        characterActionList.StartActionEvent((int)EnumPack.CharacterActionType.HarvestFarm);
    }

    private void GoToField()
    {
        var nearestField =
            extendField.GetNearestFieldWithState(transform.position + transform.forward * 20.0f, currentFarmState);

        targetPosition = nearestField ? nearestField.transform.position : extendField.transform.position;

        stateMachine.ChangeState<MovingState>();
    }

    private void GoToCave()
    {
        targetPosition = cave.GoToPos.position;
        stateMachine.ChangeState<MovingState>();
    }

    public void EmptyLayer1()
    {
        characterAnimController.Play(Constant.EMPTY, 1);
    }
}