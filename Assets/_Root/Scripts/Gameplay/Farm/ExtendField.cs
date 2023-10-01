using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ExtendField : GameComponent
{
    [SerializeField, UniqueID] private string uniqueId;
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private List<Field> fieldList = new List<Field>();
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();
    
    private ResourceConfig resourceConfig;
    private Dictionary<EnumPack.ResourceType, ResourceConfig> resourceConfigDict =
        new Dictionary<EnumPack.ResourceType, ResourceConfig>();

    public EnumPack.ResourceType ResourceType
    {
        get => Data.Load(uniqueId, EnumPack.ResourceType.None);

        set => Data.Save(uniqueId, value);
    }

    private void Awake()
    {
        foreach (var resource in resourceConfigList)
        {
            resourceConfigDict.Add(resource.resourceType, resource);
        }
    }

    private void Start()
    {
        if (ResourceType != EnumPack.ResourceType.None)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        resourceConfig = resourceConfigDict[ResourceType];

        foreach (var field in fieldList)
        {
            field.Initialize(resourceConfig);
        }
    }

    private void CalculateExtendFieldState()
    {
        fieldStateList.Reset();

        foreach (var field in fieldList)
        {
            if (field.FieldState == EnumPack.FieldState.Seedale &&
                !fieldStateList.Contains((int)EnumPack.FieldState.Seedale))
            {
                fieldStateList.Add((int)EnumPack.FieldState.Seedale);
                continue;
            }

            if (field.FieldState == EnumPack.FieldState.Waterable &&
                !fieldStateList.Contains((int)EnumPack.FieldState.Waterable))
            {
                fieldStateList.Add((int)EnumPack.FieldState.Waterable);
                continue;
            }

            if (field.FieldState == EnumPack.FieldState.Harvestable &&
                !fieldStateList.Contains((int)EnumPack.FieldState.Harvestable))
            {
                fieldStateList.Add((int)EnumPack.FieldState.Harvestable);
                continue;
            }
        }
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
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.TriggerActionFarm(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerActionFarm();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Unique ID")]
    public void ResetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        uniqueId = guid.ToString();
    }

    [ContextMenu("Setup Extend Field")]
    public void SetupExtendField()
    {
        GetFields();
        GetResources();
    }

    [ContextMenu("Get Fields")]
    public void GetFields()
    {
        fieldList = GetComponentsInChildren<Field>().ToList();
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Get Resources")]
    public void GetResources()
    {
        const string resourcesFolderPath = "Assets/_Root/Resources/ScriptableData/Resources";

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
#endif
}