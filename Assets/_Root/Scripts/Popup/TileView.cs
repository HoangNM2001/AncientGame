using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.ExLibEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

public class TileView : GameComponent
{
    [SerializeField] private Image image;
    [SerializeField] private Image icon;
    [SerializeField] private Sprite imgGround;
    [SerializeField] private Sprite imgBridge;
    [SerializeField] private Sprite imgTree;
    [SerializeField] private Sprite imgFarm;
    [SerializeField] private Sprite imgFish;
    [SerializeField] private Sprite imgHenhouse;
    [SerializeField] private Sprite imgNotUnlock;

    private EnumPack.TileType _tileType;
    private ResourceConfig _resourceConfig;
    private SaveDataElement _saveDataElement;
    private const int tileSize = 100;

    public void Initialize(Tile tile)
    {
        transform.localPosition = (Vector3Int)tile.Coord * tileSize;

        foreach (var element in tile.Elements)
        {
            if (element is not Deco)
            {
                if (element is FishingField fishingField) icon.transform.localPosition =
                (fishingField.transform.localPosition - fishingField.Tile.transform.localPosition) * tileSize / 6;
                _saveDataElement = element;
            }
        }

        if (_saveDataElement is FruitTree tree)
        {
            _tileType = EnumPack.TileType.Tree;
            _resourceConfig = tree.FruitResource;
        }
        else if (_saveDataElement is ExtendField extendField)
        {
            _tileType = EnumPack.TileType.Farm;
            _resourceConfig = extendField.ResourceConfig;
        }
        else if (_saveDataElement is FishingField)
        {
            _tileType = EnumPack.TileType.Fish;
        }
        else if (_saveDataElement is Henhouse) _tileType = EnumPack.TileType.Henhouse;
        else if (_saveDataElement is Bridge) _tileType = EnumPack.TileType.Bridge;

        if (_resourceConfig != null) icon.sprite = _resourceConfig.resourceIcon;

        icon.gameObject.SetActive(false);

        if (tile.Unlockable)
        {
            gameObject.SetActive(true);
            image.sprite = imgNotUnlock;
        }
        else
        {
            gameObject.SetActive(false);
        }

        if (tile.IsUnlocked)
        {
            if (_resourceConfig != null) icon.gameObject.SetActive(true);
            switch (_tileType)
            {
                case EnumPack.TileType.Ground:
                    image.sprite = imgGround;
                    break;
                case EnumPack.TileType.Tree:
                    image.sprite = imgTree;
                    break;
                case EnumPack.TileType.Farm:
                    image.sprite = imgFarm;
                    break;
                case EnumPack.TileType.Fish:
                    icon.gameObject.SetActive(true);
                    image.sprite = imgGround;
                    icon.sprite = imgFish;
                    break;
                case EnumPack.TileType.Henhouse:
                    icon.gameObject.SetActive(true);
                    image.sprite = imgGround;
                    icon.sprite = imgHenhouse;
                    break;
                case EnumPack.TileType.Bridge:
                    image.sprite = imgBridge;
                    break;

            }
        }
    }
}
