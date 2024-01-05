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
using UnityEditor.Experimental.GraphView;

public class Tile : SaveDataElement
{
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private TileLand land;
    [SerializeField] private GameObject unlockGroup;
    [SerializeField] private GameObject buildFx;
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    [SerializeField] private Trigger trigger;
    [SerializeField] private TextMeshPro unlockCostText;
    [SerializeField] private TextMeshPro requireText;
    [SerializeField] private Tile requiredTile;
    [SerializeField] private int requireLevel = 1;
    [SerializeField] private int unlockCost = 100;
    [SerializeField] private bool isUnlocked = false;

    private bool isShowUI;
    private bool isUnlockable;
    private MapController map;
    private List<Tile> tilesAroundList = new List<Tile>();
    private bool IsMeetRequiredLevel => playerLevel.Level >= requireLevel;
    private bool IsAnyTileAroundUnlocked => tilesAroundList.Any(t => t.IsUnlocked);

    public Action OnUnlocked;
    public Vector2Int Coord { get; private set; }
    public List<SaveDataElement> Elements { get; set; }

    public override bool IsUnlocked
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
        Debug.LogError(uniqueId + " - " + IsUnlocked + " + ");
        UpdateTextRequireLv();

        isUnlockable = IsUnlockable();
        isShowUI = IsShowUI();

        ToggleUnlockUI(isShowUI);
        ToggleTrigger(!IsUnlocked);
        ToggleObstacle(!IsUnlocked || !isUnlockable);

        if (!isUnlockable)
        {
            playerLevel.LevelChangedEvent += OnLevelChanged;
            if (requiredTile) requiredTile.OnUnlocked += CheckUnlockable;
            foreach (var tile in tilesAroundList) tile.OnUnlocked += CheckUnlockable;
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

    [ContextMenu("Unlock")]
    public void Unlock()
    {
        if (IsUnlocked) return;

        IsUnlocked = true;
        Debug.LogError(uniqueId + " - " + IsUnlocked);

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

        OnUnlocked?.Invoke();
    }

    public void UpdateLandModel()
    {
        if (IsUnlocked) land.UpdateModel();
    }

    private void CheckUnlockable()
    {
        if (!IsUnlocked)
        {
            isUnlockable = IsUnlockable();
            isShowUI = IsShowUI();

            if (isUnlockable)
            {
                ToggleTrigger(true);
                ToggleObstacle(true);
            }

            if (isShowUI) ToggleUnlockUI(true);

            if (isUnlockable)
            {
                playerLevel.LevelChangedEvent -= OnLevelChanged;
                if (requiredTile) requiredTile.OnUnlocked -= CheckUnlockable;
                foreach (var tile in tilesAroundList) tile.OnUnlocked -= CheckUnlockable;
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

    }

    private void ToggleObstacle(bool status)
    {
        navMeshObstacle.gameObject.SetActive(status);
    }

    private void UpdateTextRequireLv()
    {
        if (IsMeetRequiredLevel) requireText.gameObject.SetActive(false);
        else requireText.text = $"Require Lv {requireLevel}";
    }

    private void GetTilesAround()
    {
        if (map.TileDict.TryGetValue(Coord + Vector2Int.up, out var upTile)) tilesAroundList.Add(upTile);
        if (map.TileDict.TryGetValue(Coord + Vector2Int.down, out var downTile)) tilesAroundList.Add(downTile);
        if (map.TileDict.TryGetValue(Coord + Vector2Int.left, out var leftTile)) tilesAroundList.Add(leftTile);
        if (map.TileDict.TryGetValue(Coord + Vector2Int.right, out var rightTile)) tilesAroundList.Add(rightTile);
    }

    public void CalculateCoord(MapController mapController, float tileSize)
    {
        map = mapController;
        Coord = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.z / tileSize));
    }

    private bool IsShowUI()
    {
        return !IsUnlocked && IsAnyTileAroundUnlocked && (requiredTile == null || requiredTile.IsUnlocked);
    }

    private bool IsUnlockable()
    {
        return IsUnlocked && IsAnyTileAroundUnlocked && IsMeetRequiredLevel && (requiredTile == null || requiredTile.IsUnlocked);
    }
}
