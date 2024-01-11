using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pancake;
using UnityEditor;
using UnityEngine;

public class MapController : GameComponent
{
    private const float tileSize = 6;

    public Dictionary<Vector2Int, Tile> TileDict { get; private set; }
    public List<Tile> Tiles { get; private set; }
    public List<SaveDataElement> Elements { get; private set; }

    private void Awake()
    {
        TileDict = new Dictionary<Vector2Int, Tile>();
        Tiles = GetComponentsInChildren<Tile>().ToList();
        Elements = GetComponentsInChildren<SaveDataElement>().ToList();

        foreach (var tile in Tiles)
        {
            tile.Elements = new List<SaveDataElement>();
            tile.CalculateCoord(this, tileSize);
            TileDict.Add(tile.Coord, tile);
        }

        foreach (var element in Elements)
        {
            if (element as Tile || element as Field) continue;
            var elementPos = element.transform.position;
            elementPos.y = 0;

            var index = SimpleMath.GetNearestIndex(elementPos, Tiles.Select(t => new Vector3(t.transform.position.x, 0.0f, t.transform.position.z)).ToArray());
            Tiles[index].Elements.Add(element);
        }

        foreach (var tile in Tiles)
        {
            tile.InitTiles();
        }
    }

    private void Start()
    {
        Activate();
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        foreach (var tile in Tiles)
        {
            tile.Activate();
        }

        foreach (var tile in Tiles)
        {
            tile.UpdateLandModel();
        }

        if (LoadingScreen.IsExist)
        {
            DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => LoadingScreen.Instance.OnLoadingFinished());
        }
    }

#if  UNITY_EDITOR
    public void RenameAllTiles()
    {
        Tiles = GetComponentsInChildren<Tile>().ToList();

        for (var i = 0; i < Tiles.Count; i++)
        {
            Tiles[i].gameObject.name = $"Tile: {i}";
        }
    }
#endif
}

[CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    private MapController _mapController;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _mapController = (MapController)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("ChangeTileName"))
        {
            _mapController.RenameAllTiles();
        }
    }
}
