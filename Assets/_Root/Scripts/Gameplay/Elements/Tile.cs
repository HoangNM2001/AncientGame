using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pancake;
using UnityEngine.AI;
using TMPro;
using UnityEngine.Tilemaps;
using System.Linq;
using UnityEngine.UI;
using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using Pancake.SceneFlow;

public class Tile : SaveDataElement
{
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private TileLand land;
    [SerializeField] private GameObject unlockGroup;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private Trigger trigger;
    [SerializeField] private TextMeshPro unlockCostText;
    [SerializeField] private TextMeshPro requireText;
    [SerializeField] private Tile requiredTile;
    [SerializeField] private int requireLevel = 1;
    [SerializeField] private int unlockCost = 100;
    [SerializeField] private bool isUnlocked = false;

    private bool _isShowUI;
    private bool _isUnlockAble;
    private MapController _map;
    private readonly List<Tile> _tilesAroundList = new();
    private bool IsMeetRequiredLevel => playerLevel.Level >= requireLevel;
    private bool IsAnyTileAroundUnlocked => _tilesAroundList.Any(t => t.IsUnlocked);
    private Action _onUnlocked;

    public Vector2Int Coord { get; private set; }
    public int UnlockCost => unlockCost;
    public List<SaveDataElement> Elements { get; set; }

    protected override bool IsUnlocked
    {
        get => Data.Load(uniqueId + "_isUnlocked", isUnlocked);
        set
        {
            Data.Save(uniqueId + "_isUnlocked", value);
            Data.SaveAll();
        }
    }

    public void InitTiles()
    {
        GetTilesAround();
    }

    public override void Activate(bool restore = true)
    {
        UpdateTextRequireLv();

        _isUnlockAble = IsUnlockable();
        _isShowUI = IsShowUI();

        ToggleUnlockUI(_isShowUI);
        ToggleTrigger(!IsUnlocked);
        ToggleObstacle(!IsUnlocked || !_isUnlockAble);

        if (!_isUnlockAble)
        {
            playerLevel.LevelChangedEvent += OnLevelChanged;
            if (requiredTile) requiredTile._onUnlocked += CheckUnlockable;
            foreach (var tile in _tilesAroundList) tile._onUnlocked += CheckUnlockable;
        }

        if (IsUnlocked)
        {
            land.Activate();
            foreach (var element in Elements)
            {
                element.Activate();
            }
        }
        else
        {
            land.Deactivate();
            foreach (var element in Elements)
            {
                element.Deactivate();
            }
        }
    }

    public override void Deactivate()
    {
        foreach (var element in Elements)
        {
            element.Deactivate();
        }
    }

    public void Unlock()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;

        ToggleUnlockUI(false);
        ToggleTrigger(false);
        ToggleObstacle(false);

        land.Activate(false, () =>
        {
            foreach (var element in Elements)
            {
                element.Activate(false);
            }
        });

        _onUnlocked?.Invoke();
    }

    public void UpdateLandModel()
    {
        if (IsUnlocked) land.UpdateModel();
    }

    private void CheckUnlockable()
    {
        if (!IsUnlocked)
        {
            _isUnlockAble = IsUnlockable();
            _isShowUI = IsShowUI();

            if (_isUnlockAble)
            {
                ToggleTrigger(true);
                ToggleObstacle(true);
            }

            if (_isShowUI) ToggleUnlockUI(true);

            if (_isUnlockAble)
            {
                playerLevel.LevelChangedEvent -= OnLevelChanged;
                if (requiredTile) requiredTile._onUnlocked -= CheckUnlockable;
                foreach (var tile in _tilesAroundList) tile._onUnlocked -= CheckUnlockable;
            }
        }
    }

    private void OnLevelChanged()
    {
        UpdateTextRequireLv();
        CheckUnlockable();
    }

    private void ToggleUnlockUI(bool status)
    {
        unlockGroup.gameObject.SetActive(status);
    }

    private void ToggleTrigger(bool status)
    {
        trigger.EnterTriggerEvent -= OnTriggerEnterEvent;
        trigger.ExitTriggerEvent -= OnTriggerExitEvent;
        trigger.gameObject.SetActive(false);

        if (status)
        {
            trigger.gameObject.SetActive(true);
            trigger.EnterTriggerEvent += OnTriggerEnterEvent;
            trigger.ExitTriggerEvent += OnTriggerExitEvent;
        }
    }

    private void ToggleObstacle(bool status)
    {
        navMeshObstacle.gameObject.SetActive(status);
    }

    private void OnTriggerEnterEvent(Collider collider)
    {
        var characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(collider);
        if (characterHandleTrigger && _isUnlockAble)
        {
            characterHandleTrigger.TriggerBuilding(gameObject);
        }
    }

    private void OnTriggerExitEvent(Collider collider)
    {
        var characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(collider);
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerAction();
    }

    private void UpdateTextRequireLv()
    {
        if (IsMeetRequiredLevel) requireText.gameObject.SetActive(false);
        else requireText.text = $"Require Lv {requireLevel}";
    }

    private void GetTilesAround()
    {
        if (_map.TileDict.TryGetValue(Coord + Vector2Int.up, out var upTile)) _tilesAroundList.Add(upTile);
        if (_map.TileDict.TryGetValue(Coord + Vector2Int.down, out var downTile)) _tilesAroundList.Add(downTile);
        if (_map.TileDict.TryGetValue(Coord + Vector2Int.left, out var leftTile)) _tilesAroundList.Add(leftTile);
        if (_map.TileDict.TryGetValue(Coord + Vector2Int.right, out var rightTile)) _tilesAroundList.Add(rightTile);
    }

    public void CalculateCoord(MapController mapController, float tileSize)
    {
        _map = mapController;
        Coord = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize),
            Mathf.RoundToInt(transform.position.z / tileSize));
    }

    private bool IsShowUI()
    {
        return !IsUnlocked && IsAnyTileAroundUnlocked && (requiredTile == null || requiredTile.IsUnlocked);
    }

    private bool IsUnlockable()
    {
        return IsUnlocked || (IsAnyTileAroundUnlocked && IsMeetRequiredLevel &&
                              (requiredTile == null || requiredTile.IsUnlocked));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        var guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.blue;

        var position = transform.position;

        Handles.Label(position + Vector3.up * 0.5f + Vector3.forward * 0.75f, gameObject.name, guiStyle);

        if (requiredTile)
        {
            var dir = requiredTile.transform.position - position;
            GizmosUtils.GizmosArrow(position + Vector3.up, dir, Color.red, 1);
        }
    }
#endif
}