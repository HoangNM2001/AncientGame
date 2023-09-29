using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using DG.Tweening;
using Pancake;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Field : GameComponent
{
    [SerializeField, UniqueID] private string uniqueId;
    [SerializeField] private MeshRenderer fieldRenderer;
    [SerializeField] private Color soilColor;
    [SerializeField] private Color seededColor;
    [SerializeField] private Color wateredColor;
    [SerializeField] private GameObjectPool leavesParticlePool;
    
    private ResourceConfig resourceConfig;
    private MaterialPropertyBlock fieldMaterialBlock;
    private GameObject smallTree;
    private GameObject bigTree;
    private GameObjectPool flyModelPool;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    private const float SeedDuration = 0.5f;
    private const float WaterDuration = 0.8f;
    private const float HarvestDuration = 5.0f;
    private const int MaxFlyModel = 4;

    public EnumPack.FieldState FieldState
    {
        get => Data.Load(uniqueId, EnumPack.FieldState.Seedale);
        private set => Data.Save(uniqueId, value);
    }

    private void Awake()
    {
        fieldMaterialBlock = new MaterialPropertyBlock();
        fieldRenderer.GetPropertyBlock(fieldMaterialBlock);
    }

    public void Initialize(ResourceConfig newResource)
    {
        resourceConfig = newResource;
        smallTree = Instantiate(resourceConfig.smallTree, transform);
        bigTree = Instantiate(resourceConfig.bigTree, transform);
        flyModelPool = resourceConfig.flyModelPool;
        
        smallTree.transform.RandomLocalRotation(true);
        bigTree.transform.RandomLocalRotation(true);
        smallTree.SetActive(false);
        bigTree.SetActive(false);
        
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

    public void DoFarming(EnumPack.CharacterActionType actionType)
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
                DoHarvest();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(actionType), actionType, null);
        }
    }

    private void DoSeed()
    {
        if (FieldState != EnumPack.FieldState.Seedale) return;
        FieldState = EnumPack.FieldState.Waterable;
        
        smallTree.SetActive(true);
        smallTree.transform.localScale = Vector3.zero;
        smallTree.transform.DOScale(Vector3.one, SeedDuration).SetTarget(smallTree);
        ChangeFieldColor(soilColor, seededColor, SeedDuration);
    }

    private void DoWater()
    {
        if (FieldState != EnumPack.FieldState.Waterable) return;
        FieldState = EnumPack.FieldState.Harvestable;
        
        smallTree.SetActive(false);
        bigTree.SetActive(true);
        bigTree.transform.localScale = Vector3.zero;
        bigTree.transform.DOScale(Vector3.one, WaterDuration).SetEase(Ease.OutBack).SetTarget(bigTree);
        ChangeFieldColor(seededColor, wateredColor, WaterDuration);
    }

    private void DoHarvest()
    {
        if (FieldState != EnumPack.FieldState.Harvestable) return;
        FieldState = EnumPack.FieldState.Seedale;
        
        bigTree.SetActive(false);

        GameObject tempLeaves = leavesParticlePool.Request();
        float tempYPos = tempLeaves.transform.localPosition.y;
        tempLeaves.transform.SetParent(transform);
        tempLeaves.transform.localPosition = new Vector3(0.0f, tempYPos, 0.0f);
        tempLeaves.GetComponent<LeavesParticle>().ChangeParticleColor(resourceConfig.treeColor);
        DOTween.Sequence().AppendInterval(2.0f).AppendCallback(() => leavesParticlePool.Return(tempLeaves));
        
        ChangeFieldColor(wateredColor, soilColor, HarvestDuration);
        
        int randomFlyModel = Random.Range(1, MaxFlyModel);
        for (int i = 1; i <= randomFlyModel; i++)
        {
            GameObject tempFly = flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = Vector3.zero;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() => flyModelPool.Return(tempFly));
        }
    }

    private void SetSeededState()
    {
        smallTree.gameObject.SetActive(true);
        bigTree.gameObject.SetActive(false);
        fieldMaterialBlock.SetColor(ShaderColor, seededColor);
        fieldRenderer.SetPropertyBlock(fieldMaterialBlock);
    }

    private void SetWateredState()
    {
        smallTree.gameObject.SetActive(false);
        bigTree.gameObject.SetActive(true);
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

#if UNITY_EDITOR
    [ContextMenu("Reset Unique Id")]
    public void ResetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        uniqueId = guid.ToString();
    }
#endif
}