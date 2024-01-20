using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;

public class MapController : GameComponent
{
    [SerializeField] private ScriptableListTile tileList;

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
            tileList.Add(tile);
        }
    }

    private void Start()
    {
        Activate();
    }

    private void Activate()
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

        foreach (var tile in Tiles)
        {
            var coords = new Vector2Int(Mathf.RoundToInt(tile.transform.position.x / tileSize),
                                        Mathf.RoundToInt(tile.transform.position.z / tileSize));
            tile.gameObject.name = $"Tile {coords}";
        }

        Elements = GetComponentsInChildren<SaveDataElement>().ToList();

        foreach (var element in Elements)
        {
            if (element as Tile) continue;

            var coords = new Vector2Int(Mathf.RoundToInt(element.transform.position.x / tileSize),
                                        Mathf.RoundToInt(element.transform.position.z / tileSize));

            var oldName = element.gameObject.name;
            var index = oldName.IndexOf('(');
            if (index != -1) 
            {
                element.gameObject.name = oldName.Substring(0, index);
            }
            element.gameObject.name = $"{element.gameObject.name}{coords}";
        }
    }
#endif
}

#if  UNITY_EDITOR
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
#endif
