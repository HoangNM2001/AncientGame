using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;

public class PlayerController : GameComponent
{
    [SerializeField] private PlayerControllerVariable playerVariable;
    [SerializeField] private IntVariable goldVariable;
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private PlayerStat playerStat;
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private PlayerActionList playerActionList;
    [SerializeField] private CharacterHandleTrigger characterHandleTrigger;
    [SerializeField] private Vector3Variable playerPosition;
    [SerializeField] private Vector3Variable playerRotation;
    [SerializeField] private GameObject buildHammer;
    [SerializeField] private GameObject buildFx;
    [SerializeField] private GameObject levelUpFx;
    [SerializeField] private GameObjectPool flyTextPool;
    [SerializeField] private Upgradable moveUpgradable;
    [SerializeField] private Upgradable workUpgradable;
    [SerializeField] private List<ResourceConfig> resourceConfigList;

    private NavmeshController _navmeshController;
    private PlayerHandleInput _playerHandleInput;
    private EnumPack.ControlType _controlType;
    private float _currentMoveSpeed;
    private bool _isBuilding;
    private bool _moveByBike;
    private Tile _currentTile;

    public PlayerStat PlayerStat => playerStat;
    public CharacterAnimController CharacterAnimController => characterAnimController;
    private bool CanBuild => playerActionList.CurrentCharacterAction == null;

    private void Awake()
    {
        _navmeshController = GetComponent<NavmeshController>();
        _playerHandleInput = GetComponent<PlayerHandleInput>();

        // transform.position = playerPosition.Value;
        // transform.rotation = Quaternion.Euler(playerRotation.Value);
    }

    protected override void OnEnabled()
    {
        getCharacterEvent.OnRaised += getCharacterEvent_OnRaised;
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
        moveUpgradable.OnUpgraded += moveUpgradable_OnUpgraded;
        workUpgradable.OnUpgraded += workUpgradable_OnUpgraded;
        playerLevel.OnLevelChangedEvent += playerLevel_OnLevelChangedEvent;
        // playerLevel.OnExpChangedEvent += playerLevel_OnExpChangedEvent;
    }

    private void moveUpgradable_OnUpgraded()
    {
        levelUpFx.SetActive(true);
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => levelUpFx.SetActive(false));
        UpdateMoveSpeed();
        ShowFlyText($"{moveUpgradable.desc} UP");
    }

    private void workUpgradable_OnUpgraded()
    {
        levelUpFx.SetActive(true);
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => levelUpFx.SetActive(false));
        ShowFlyText($"{workUpgradable.desc} UP");
    }

    private GameObject getCharacterEvent_OnRaised()
    {
        return gameObject;
    }

    private void changeInputEvent_OnRaised(int newControlType)
    {
        _controlType = (EnumPack.ControlType)newControlType;
        _navmeshController.ResetPath();
    }

    private void playerLevel_OnExpChangedEvent(float value)
    {
        ShowFlyText($"+ {playerLevel.ExpUp} Exp");
    }

    private void playerLevel_OnLevelChangedEvent(int level)
    {
        characterHandleTrigger.ShowPopupUpgrade();
        ShowFlyText("Level up");
    }

    protected override void OnDisabled()
    {
        getCharacterEvent.OnRaised -= getCharacterEvent_OnRaised;
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
        moveUpgradable.OnUpgraded -= moveUpgradable_OnUpgraded;
        workUpgradable.OnUpgraded -= workUpgradable_OnUpgraded;
        playerLevel.OnLevelChangedEvent += playerLevel_OnLevelChangedEvent;
        // playerLevel.OnExpChangedEvent -= playerLevel_OnExpChangedEvent;
    }

    private void Start()
    {
        UpdateMoveSpeed();
        _controlType = EnumPack.ControlType.Move;
        playerVariable.Value = this;
    }

    protected override void Tick()
    {
        if (_isBuilding) return;
        if (_controlType != EnumPack.ControlType.Move) return;

        _playerHandleInput.GetInput();
        MoveByDirection(_playerHandleInput.MoveDir.normalized, _currentMoveSpeed, Time.deltaTime);

        // playerPosition.Value = transform.position;
        // playerRotation.Value = transform.rotation.eulerAngles;
    }

    private void ShowFlyText(string text, Action completeAction = null)
    {
        var flyText = flyTextPool.Request().GetComponent<FlyText>();
        flyText.transform.position = transform.localPosition + Vector3.up * 3.0f;
        flyText.Initialize(text);
        flyText.Show(false, completeAction);
    }

    public void PunishOnLose()
    {
        foreach (var resource in resourceConfigList)
        {
            if (resource.resourceQuantity.Value > 0)
            {
                var isPunish = UnityEngine.Random.Range(0, 2);
                Debug.LogError(isPunish);
                if (isPunish == 0) continue;

                var punishCount = UnityEngine.Random.Range(0, resource.resourceQuantity.Value);
                punishCount = Mathf.Clamp(punishCount, 0, 10);
                Debug.LogError(punishCount);
                var randomFlyModel = UnityEngine.Random.Range(2, 5);

                for (var i = 1; i <= randomFlyModel; i++)
                {
                    var tempFly = resource.flyModelPool.Request();
                    tempFly.transform.SetParent(transform);
                    tempFly.transform.localPosition = Vector3.zero;
                    tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
                    {
                        resource.flyModelPool.Return(tempFly);
                    });
                }

                resource.resourceQuantity.Value -= punishCount;
            }
        }
    }

    public void CheckToBuild()
    {
        if (!CanBuild || !characterHandleTrigger.CurrentInteract.TryGetComponent<Tile>(out var tile)) return;
        if (goldVariable.Value <= tile.UnlockCost) return;
        _currentTile = tile;
        Build();
    }

    private void Build()
    {
        _isBuilding = true;
        goldVariable.Value -= _currentTile.UnlockCost;
        buildHammer.SetActive(true);
        buildFx.transform.position = _currentTile.transform.position;
        buildFx.SetActive(true);
        characterAnimController.Play(Constant.BUIDLING, 0);

        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(StopBuild);
    }

    private void StopBuild()
    {
        _isBuilding = false;
        _currentTile.Unlock();
        buildHammer.SetActive(false);
        buildFx.SetActive(false);
        BackToMove();
    }

    private void BackToMove()
    {
        characterAnimController.Play(_moveByBike ? Constant.IDLE_2_BIKE : Constant.IDLE_2_RUN, 0);
    }

    private void MoveByDirection(Vector3 direction, float moveSpeed, float deltaTime)
    {
        _navmeshController.MoveByDirection(direction, moveSpeed, playerStat.RotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(_navmeshController.VelocityRatio, deltaTime);
    }

    public void MoveToPosition(Vector3 position, float moveSpeed, float deltaTime)
    {
        _navmeshController.MoveByPosition(position, 0.0f, moveSpeed, playerStat.RotateSpeed, 0.1f, deltaTime);
        characterAnimController.UpdateIdle2Run(_navmeshController.VelocityRatio, deltaTime);
    }

    public void RotateToTarget(Vector3 pos, float deltaTime)
    {
        var dir = pos - transform.position;
        dir.y = 0.0f;
        _navmeshController.MoveByDirection(dir, 0, playerStat.RotateSpeed, deltaTime);
        characterAnimController.UpdateIdle2Run(_navmeshController.VelocityRatio, deltaTime);
    }

    public void UpdateWorkingMoveSpeed()
    {
        _currentMoveSpeed = playerStat.WorkMoveSpeed;
    }

    public void UpdateMoveSpeed()
    {
        _currentMoveSpeed = playerStat.MoveSpeed;
    }

#if UNITY_EDITOR
    [ContextMenu("Get Farm Resources")]
    public void GetFarmResources()
    {
        const string resourcesFolderPath = "Assets/_Root/ScriptableData/ResourceConfigs/FarmResources";

        var resourcePaths = AssetDatabase.FindAssets("t:ResourceConfig", new string[] { resourcesFolderPath });

        var resourceConfigs = new ResourceConfig[resourcePaths.Length];

        for (var i = 0; i < resourcePaths.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(resourcePaths[i]);
            resourceConfigs[i] = AssetDatabase.LoadAssetAtPath<ResourceConfig>(assetPath);
        }

        resourceConfigList = resourceConfigs.ToList();
        EditorUtility.SetDirty(this);
    }
#endif
}