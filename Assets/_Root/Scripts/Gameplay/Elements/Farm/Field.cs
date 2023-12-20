using System;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;
using Random = UnityEngine.Random;

public class Field : SaveDataElement
{
    [SerializeField] private MeshRenderer fieldRenderer;
    [SerializeField] private Color soilColor;
    [SerializeField] private Color seededColor;
    [SerializeField] private Color wateredColor;
    [SerializeField] private GameObjectPool leavesParticlePool;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    private ExtendField parentField;
    private ResourceConfig resourceConfig;
    private MaterialPropertyBlock fieldMaterialBlock;
    private GameObject smallTree;
    private GameObject bigTree;
    private GameObjectPool smallTreePool;
    private GameObjectPool bigTreePool;
    private GameObjectPool flyModelPool;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    private const float SeedDuration = 0.5f;
    private const float WaterDuration = 0.8f;
    private const float HarvestDuration = 5.0f;
    private const int MaxFlyModel = 4;

    public EnumPack.FieldState FieldState
    {
        get => Data.Load(uniqueId + "FieldState", EnumPack.FieldState.Seedale);
        private set => Data.Save(uniqueId + "FieldState", value);
    }

    private void Awake()
    {
        fieldMaterialBlock = new MaterialPropertyBlock();
        fieldRenderer.GetPropertyBlock(fieldMaterialBlock);
    }

    public void Initialize(ExtendField extendField, ResourceConfig newResource)
    {
        parentField = extendField;
        resourceConfig = newResource;

        smallTreePool = resourceConfig.smallTreePool;
        bigTreePool = resourceConfig.bigTreePool;
        flyModelPool = resourceConfig.flyModelPool;

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

        smallTree = smallTreePool.Request();
        smallTree.transform.SetParent(transform);
        smallTree.transform.localPosition = Vector3.zero;
        smallTree.transform.RandomLocalRotation(true);
        smallTree.transform.localScale = Vector3.zero;
        smallTree.transform.DOScale(Vector3.one, SeedDuration).SetTarget(smallTree);
        ChangeFieldColor(soilColor, seededColor, SeedDuration);

        parentField.DoSeed();
    }

    private void DoWater()
    {
        if (FieldState != EnumPack.FieldState.Waterable) return;
        FieldState = EnumPack.FieldState.Harvestable;

        smallTreePool.Return(smallTree);

        bigTree = bigTreePool.Request();
        bigTree.transform.SetParent(transform);
        bigTree.transform.localPosition = Vector3.zero;
        bigTree.transform.RandomLocalRotation(true);
        bigTree.transform.localScale = Vector3.zero;
        bigTree.transform.DOScale(Vector3.one, WaterDuration).SetEase(Ease.OutBack).SetTarget(bigTree);
        ChangeFieldColor(seededColor, wateredColor, WaterDuration);

        parentField.DoWater();
    }

    private void DoHarvest(bool isPlayer)
    {
        if (FieldState != EnumPack.FieldState.Harvestable) return;
        FieldState = EnumPack.FieldState.Seedale;

        bigTreePool.Return(bigTree);

        var tempLeaves = leavesParticlePool.Request();
        var tempYPos = tempLeaves.transform.localPosition.y;
        tempLeaves.transform.SetParent(transform);
        tempLeaves.transform.localPosition = new Vector3(0.0f, tempYPos, 0.0f);
        tempLeaves.GetComponent<LeavesParticle>().ChangeParticleColor(resourceConfig.treeColor);
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => leavesParticlePool.Return(tempLeaves));

        ChangeFieldColor(wateredColor, soilColor, HarvestDuration);

        var randomFlyModel = Random.Range(1, MaxFlyModel);

        for (var i = 1; i <= randomFlyModel; i++)
        {
            var tempFly = flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = Vector3.zero;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                flyModelPool.Return(tempFly);
                if (isPlayer)
                {
                    flyUIEvent.Raise(new FlyEventData
                    {
                        resourceType = resourceConfig.resourceType,
                        worldPos = tempFly.transform.position
                    });
                }
            });
        }

        parentField.DoHarvest();
    }

    private void SetSeededState()
    {
        smallTree = smallTreePool.Request();
        smallTree.transform.SetParent(transform);
        smallTree.transform.localPosition = Vector3.zero;
        smallTree.transform.RandomLocalRotation(true);

        fieldMaterialBlock.SetColor(ShaderColor, seededColor);
        fieldRenderer.SetPropertyBlock(fieldMaterialBlock);
    }

    private void SetWateredState()
    {
        bigTree = bigTreePool.Request();
        bigTree.transform.SetParent(transform);
        bigTree.transform.localPosition = Vector3.zero;
        bigTree.transform.RandomLocalRotation(true);

        fieldMaterialBlock.SetColor(ShaderColor, wateredColor);
        fieldRenderer.SetPropertyBlock(fieldMaterialBlock);
    }

    private void ChangeFieldColor(Color fromColor, Color toColor, float duration)
    {
        DOTween.Kill(fieldRenderer);
        DOTween.To(() => 0, x =>
        {
            fieldMaterialBlock.SetColor(ShaderColor, Color.Lerp(fromColor, toColor, x));
            fieldRenderer.SetPropertyBlock(fieldMaterialBlock);
        }, 1.0f, duration).SetTarget(fieldRenderer);
    }
}