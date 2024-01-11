using System;
using DG.Tweening;
using Pancake;
using Pancake.Apex;
using Pancake.Scriptable;
using UnityEngine;
using UnityEngine.Experimental.Playables;
using Random = UnityEngine.Random;

public class Field : SaveDataElement
{
    [SerializeField] private MeshRenderer fieldRenderer;
    [SerializeField] private Color soilColor;
    [SerializeField] private Color seededColor;
    [SerializeField] private Color wateredColor;
    [SerializeField] private GameObjectPool leavesParticlePool;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    private Collider _collider;
    private ExtendField _parentField;
    private ResourceConfig _resourceConfig;
    private MaterialPropertyBlock _fieldMaterialBlock;
    private GameObject _smallTree;
    private GameObject _bigTree;
    private GameObjectPool _smallTreePool;
    private GameObjectPool _bigTreePool;
    private GameObjectPool _flyModelPool;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    private const float SeedDuration = 0.5f;
    private const float WaterDuration = 0.8f;
    private const float HarvestDuration = 5.0f;
    private const int MaxFlyModel = 4;

    public EnumPack.FieldState FieldState
    {
        get => Data.Load($"{uniqueId}_fieldState", EnumPack.FieldState.Seedale);
        private set => Data.Save($"{uniqueId}_fieldState", value);
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _fieldMaterialBlock = new MaterialPropertyBlock();
        fieldRenderer.GetPropertyBlock(_fieldMaterialBlock);
    }

    public void Initialize(ExtendField extendField, ResourceConfig newResource)
    {
        _parentField = extendField;
        _resourceConfig = newResource;

        _smallTreePool = _resourceConfig.smallTreePool;
        _bigTreePool = _resourceConfig.bigTreePool;
        _flyModelPool = _resourceConfig.flyModelPool;

        InitFieldState();
    }

    private void InitFieldState()
    {
        switch (FieldState)
        {
            case EnumPack.FieldState.Seedale:
                break;
            case EnumPack.FieldState.Waterable:
                SetSeededState();
                break;
            case EnumPack.FieldState.Harvestable:
                SetWateredState();
                break;
        }
    }

    public void DoFarming(EnumPack.CharacterActionType actionType, bool isPlayer)
    {
        switch (actionType)
        {
            case EnumPack.CharacterActionType.SeedFarm:
                DoSeed();
                break;
            case EnumPack.CharacterActionType.WaterFarm:
                DoWater();
                break;
            case EnumPack.CharacterActionType.HarvestFarm:
                DoHarvest(isPlayer);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
    }

    private void DoSeed()
    {
        if (FieldState != EnumPack.FieldState.Seedale) return;
        FieldState = EnumPack.FieldState.Waterable;

        _smallTree = _smallTreePool.Request();
        _smallTree.transform.SetParent(transform);
        _smallTree.transform.localPosition = Vector3.zero;
        _smallTree.transform.RandomLocalRotation(true);
        _smallTree.transform.localScale = Vector3.zero;
        _smallTree.transform.DOScale(Vector3.one, SeedDuration).SetTarget(_smallTree);
        ChangeFieldColor(soilColor, seededColor, SeedDuration);

        _parentField.DoSeed();
    }

    private void DoWater()
    {
        if (FieldState != EnumPack.FieldState.Waterable) return;
        FieldState = EnumPack.FieldState.Harvestable;

        _smallTreePool.Return(_smallTree);

        _bigTree = _bigTreePool.Request();
        _bigTree.transform.SetParent(transform);
        _bigTree.transform.localPosition = Vector3.zero;
        _bigTree.transform.RandomLocalRotation(true);
        _bigTree.transform.localScale = Vector3.zero;
        _bigTree.transform.DOScale(Vector3.one, WaterDuration).SetEase(Ease.OutBack).SetTarget(_bigTree);
        ChangeFieldColor(seededColor, wateredColor, WaterDuration);

        _parentField.DoWater();
    }

    private void DoHarvest(bool isPlayer)
    {
        if (FieldState != EnumPack.FieldState.Harvestable) return;
        FieldState = EnumPack.FieldState.Seedale;

        _bigTreePool.Return(_bigTree);

        var tempLeaves = leavesParticlePool.Request();
        var tempYPos = tempLeaves.transform.localPosition.y;
        tempLeaves.transform.SetParent(transform);
        tempLeaves.transform.localPosition = new Vector3(0.0f, tempYPos, 0.0f);
        tempLeaves.GetComponent<LeavesParticle>().ChangeParticleColor(_resourceConfig.treeColor);
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => leavesParticlePool.Return(tempLeaves));

        ChangeFieldColor(wateredColor, soilColor, HarvestDuration);

        var randomFlyModel = Random.Range(1, MaxFlyModel);

        for (var i = 1; i <= randomFlyModel; i++)
        {
            var tempFly = _flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = Vector3.zero;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                _flyModelPool.Return(tempFly);
                if (isPlayer)
                {
                    flyUIEvent.Raise(new FlyEventData
                    {
                        resourceType = _resourceConfig.resourceType,
                        worldPos = tempFly.transform.position
                    });
                }
            });
        }

        if (isPlayer) _resourceConfig.resourceQuantity.Value += randomFlyModel;

        _parentField.DoHarvest(isPlayer);
    }

    private void SetSeededState()
    {
        _smallTree = _smallTreePool.Request();
        _smallTree.transform.SetParent(transform);
        _smallTree.transform.localPosition = Vector3.zero;
        _smallTree.transform.RandomLocalRotation(true);

        _fieldMaterialBlock.SetColor(ShaderColor, seededColor);
        fieldRenderer.SetPropertyBlock(_fieldMaterialBlock);
    }

    private void SetWateredState()
    {
        _bigTree = _bigTreePool.Request();
        _bigTree.transform.SetParent(transform);
        _bigTree.transform.localPosition = Vector3.zero;
        _bigTree.transform.RandomLocalRotation(true);

        _fieldMaterialBlock.SetColor(ShaderColor, wateredColor);
        fieldRenderer.SetPropertyBlock(_fieldMaterialBlock);
    }

    public void ForceGrow()
    {
        if (FieldState == EnumPack.FieldState.Harvestable) return;
        if (FieldState == EnumPack.FieldState.Waterable) _smallTreePool.Return(_smallTree);

        FieldState = EnumPack.FieldState.Harvestable;

        _bigTree = _bigTreePool.Request();
        _bigTree.transform.SetParent(transform);
        _bigTree.transform.localPosition = Vector3.zero;
        _bigTree.transform.RandomLocalRotation(true);
        _bigTree.transform.localScale = Vector3.zero;
        _bigTree.transform.DOScale(Vector3.one, WaterDuration).SetEase(Ease.OutBack).SetTarget(_bigTree);
        ChangeFieldColor(seededColor, wateredColor, WaterDuration);
    }

    public override void Activate(bool restore = true)
    {
        gameObject.SetActive(true);
        DOTween.Kill(transform);
        if (restore)
        {
            OnActivated();
        }
        else
        {
            transform.DOMoveY(0.5f, 0.15f).From().SetTarget(transform).OnComplete(OnActivated);
        }
    }

    private void OnActivated()
    {
        _collider.enabled = true;
    }

    private void ChangeFieldColor(Color fromColor, Color toColor, float duration)
    {
        DOTween.Kill(fieldRenderer);
        DOTween.To(() => 0, x =>
        {
            _fieldMaterialBlock.SetColor(ShaderColor, Color.Lerp(fromColor, toColor, x));
            fieldRenderer.SetPropertyBlock(_fieldMaterialBlock);
        }, 1.0f, duration).SetTarget(fieldRenderer);
    }
}