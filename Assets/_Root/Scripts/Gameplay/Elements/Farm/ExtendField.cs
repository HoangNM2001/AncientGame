using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ExtendField : SaveDataElement
{
    [SerializeField] private ScriptableListExtendField extendFieldList;
    [SerializeField] private ExtendField mainField;
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private List<Field> fieldList = new();
    [SerializeField] private List<ResourceConfig> resourceConfigList = new();

    private BoxCollider _collider;
    private int _canSeedCount;
    private int _canWaterCount;
    private int _canHarvestCount;
    private ResourceConfig _resourceConfig;

    public Action OnStateChange;
    public Action<bool> OnHarvest;

    public List<Field> FieldList => fieldList;
    public ResourceConfig ResourceConfig => _resourceConfig;

    private EnumPack.ResourceType ResourceType
    {
        get => Data.Load($"{Id}_resourceType", EnumPack.ResourceType.None);

        set => Data.Save($"{Id}_resourceType", value);
    }

    public EnumPack.FieldState TransferFarmState
    {
        get
        {
            if (_canHarvestCount > 0) return EnumPack.FieldState.Harvestable;
            if (_canSeedCount > 0) return EnumPack.FieldState.Seedale;
            if (_canWaterCount > 0) return EnumPack.FieldState.Waterable;
            return EnumPack.FieldState.Seedale;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        InitCount();
    }

    private void Start()
    {
        if (ResourceType != EnumPack.ResourceType.None) Initialize();
    }

    public override void Activate(bool restore = true)
    {
        extendFieldList.Add(this);
        IsUnlocked = true;
        gameObject.SetActive(true);

        foreach (var field in fieldList)
        {
            field.gameObject.SetActive(false);
        }

        if (restore)
        {
            foreach (var field in fieldList)
            {
                field.Activate();
            }

            OnActivated();
        }
        else
        {
            StartCoroutine(IEShowField());
        }
    }

    IEnumerator IEShowField()
    {
        foreach (var field in fieldList)
        {
            yield return new WaitForSeconds(0.05f);
            field.Activate(false);
        }
        OnActivated();
    }

    private void OnActivated()
    {
        if (mainField == null) return;
        if (!mainField.IsUnlocked) return;
        mainField.AppendExtendField(this);
        _collider.enabled = false;
    }

    private void AppendExtendField(ExtendField sideExtendField)
    {
        var bounds = _collider.bounds;
        bounds.Encapsulate(sideExtendField._collider.bounds);

        _collider.center = bounds.center - transform.position;
        _collider.size = bounds.size;

        fieldList.AddRange(sideExtendField.fieldList);

        if (ResourceType != EnumPack.ResourceType.None)
        {
            foreach (var field in sideExtendField.fieldList)
            {
                field.Initialize(this, _resourceConfig);
            }
        }

        _canSeedCount += sideExtendField._canSeedCount;
        _canWaterCount += sideExtendField._canWaterCount;
        _canHarvestCount += sideExtendField._canHarvestCount;

        extendFieldList.Remove(sideExtendField);
    }

    protected override void Initialize()
    {
        foreach (var resource in resourceConfigList.Where(resource => ResourceType == resource.resourceType))
        {
            _resourceConfig = resource;
            break;
        }

        foreach (var field in fieldList)
        {
            field.Initialize(this, _resourceConfig);
        }
    }

    public void InitCount()
    {
        _canSeedCount = _canWaterCount = _canHarvestCount = 0;

        foreach (var field in fieldList)
        {
            switch (field.FieldState)
            {
                case EnumPack.FieldState.Seedale:
                    _canSeedCount++;
                    break;
                case EnumPack.FieldState.Waterable:
                    _canWaterCount++;
                    break;
                case EnumPack.FieldState.Harvestable:
                    _canHarvestCount++;
                    break;
            }
        }
    }

    public void InitializeOnNewSeed(EnumPack.ResourceType newResourceType)
    {
        ResourceType = newResourceType;
        Initialize();
    }

    private void CalculateExtendFieldState()
    {
        fieldStateList.Reset();

        if (_canSeedCount > 0) fieldStateList.Add((int)EnumPack.FieldState.Seedale);
        if (_canWaterCount > 0) fieldStateList.Add((int)EnumPack.FieldState.Waterable);
        if (_canHarvestCount > 0) fieldStateList.Add((int)EnumPack.FieldState.Harvestable);
    }

    public Field GetNearestFieldWithState(Vector3 pos, EnumPack.FieldState fieldState)
    {
        Field targetField = null;
        var minDistance = 10000f;

        foreach (var field in fieldList)
        {
            if (field.FieldState == fieldState)
            {
                var distance = SimpleMath.SqrDist(pos, field.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    targetField = field;
                }
            }
        }

        return targetField;
    }

    public void DoSeed()
    {
        CheckChangeState(ref _canSeedCount, ref _canWaterCount);
        if (_canSeedCount == 0) OnStateChange?.Invoke();
    }

    public void DoWater()
    {
        CheckChangeState(ref _canWaterCount, ref _canHarvestCount);
        if (_canWaterCount == 0) OnStateChange?.Invoke();
    }

    public void DoHarvest(bool isPlayer)
    {
        CheckChangeState(ref _canHarvestCount, ref _canSeedCount);
        if (!isPlayer) OnHarvest?.Invoke(_canHarvestCount == 0);
        // if (_canHarvestCount == 0) OnStateChange?.Invoke();
    }

    private void CheckChangeState(ref int previousStateCount, ref int newStateCount)
    {
        previousStateCount--;
        newStateCount++;
    }

    public bool IsSeeded()
    {
        foreach (var field in fieldList)
        {
            if (field.FieldState != EnumPack.FieldState.Seedale)
            {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        CalculateExtendFieldState();
        if (other.TryGetComponent<IFarmer>(out var farmer)) farmer.TriggerActionFarm(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IFarmer>(out var farmer)) farmer.ExitTriggerActionFarm();
    }

#if UNITY_EDITOR
    [ContextMenu("Get Fields")]
    public void GetFields()
    {
        fieldList = GetComponentsInChildren<Field>().ToList();
        EditorUtility.SetDirty(this);
    }

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

    [ContextMenu("Setup Extend Field")]
    public void SetupExtendField()
    {
        GetFields();
        GetFarmResources();
        ResetUniqueID();
        EditorUtility.SetDirty(this);
    }
#endif
}