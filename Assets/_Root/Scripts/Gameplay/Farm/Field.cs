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
    [SerializeField] private MeshRenderer fieldRenderer;
    [SerializeField] private Color soilColor;
    [SerializeField] private Color seededColor;
    [SerializeField] private Color wateredColor;
    [SerializeField] private GameObjectPool leavesParticlePool;

    [Header("TEST")] 
    [SerializeField] private ResourceConfig resourceConfig;
    
    private EnumPack.FieldState fieldState;
    private MaterialPropertyBlock fieldMaterialBlock;
    private GameObject smallTree;
    private GameObject bigTree;
    private GameObjectPool flyModelPool;
    private static readonly int ShaderColor = Shader.PropertyToID("_Color");
    private const float SeedDuration = 0.5f;
    private const float WaterDuration = 0.8f;
    private const float HarvestDuration = 5.0f;
    private const int MaxFlyModel = 4;

    private void Awake()
    {
        fieldMaterialBlock = new MaterialPropertyBlock();
        fieldRenderer.GetPropertyBlock(fieldMaterialBlock);
    }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        smallTree = Instantiate(resourceConfig.smallTree, transform);
        bigTree = Instantiate(resourceConfig.bigTree, transform);
        flyModelPool = resourceConfig.flyModelPool;
        
        smallTree.transform.RandomLocalRotation(true);
        bigTree.transform.RandomLocalRotation(true);
        smallTree.SetActive(false);
        bigTree.SetActive(false);
        
        // Data.Save("id_field", ResourceType);
        
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
        if (fieldState != EnumPack.FieldState.Seedale) return;
        fieldState = EnumPack.FieldState.Waterable;
        
        smallTree.SetActive(true);
        smallTree.transform.localScale = Vector3.zero;
        smallTree.transform.DOScale(Vector3.one, SeedDuration).SetTarget(smallTree);
        ChangeFieldColor(soilColor, seededColor, SeedDuration);
    }

    private void DoWater()
    {
        if (fieldState != EnumPack.FieldState.Waterable) return;
        fieldState = EnumPack.FieldState.Harvestable;
        
        smallTree.SetActive(false);
        bigTree.SetActive(true);
        bigTree.transform.localScale = Vector3.zero;
        bigTree.transform.DOScale(Vector3.one, WaterDuration).SetEase(Ease.OutBack).SetTarget(bigTree);
        ChangeFieldColor(seededColor, wateredColor, WaterDuration);
    }

    private void DoHarvest()
    {
        if (fieldState != EnumPack.FieldState.Harvestable) return;
        fieldState = EnumPack.FieldState.Seedale;
        
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