using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Mono.Cecil;
using Newtonsoft.Json;
using Pancake;
using Pancake.SceneFlow;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Random = UnityEngine.Random;

public class ResourcesCave : SaveDataElement
{
    [SerializeField] private ShowableUI showableUI;
    [SerializeField] private CaveResourcesUI caveResourcesUIPrefab;
    [SerializeField] private List<ResourceConfig> caveResourceList;
    [SerializeField] private int MaxNumberOfResources;
    [SerializeField] private Transform resourceUIParent;
    [SerializeField] private Transform goToPos;
    [SerializeField] private bool isCaveFull;

    private const int CaveMaxCapacity = 50;
    private CharacterHandleTrigger characterHandleTrigger;
    private Dictionary<EnumPack.ResourceType, ResourceConfig> resourceDict;
    private Dictionary<EnumPack.ResourceType, CaveResourcesUI> caveResourceUIDict;
    private Dictionary<EnumPack.ResourceType, int> resourceCapacityDict;
    private string ResourceCapacityJson
    {
        get => Data.Load(uniqueId, "");
        set => Data.Save(uniqueId, value);
    }

    public Transform GoToPos => goToPos;
    public List<ResourceConfig> CaveResourceList => caveResourceList;

    private void Awake()
    {
        caveResourceUIDict = new Dictionary<EnumPack.ResourceType, CaveResourcesUI>();
        resourceCapacityDict = JsonConvert.DeserializeObject<Dictionary<EnumPack.ResourceType, int>>(ResourceCapacityJson) ?? new Dictionary<EnumPack.ResourceType, int>();
        resourceDict = new Dictionary<EnumPack.ResourceType, ResourceConfig>();
        
        foreach (var resource in caveResourceList)
        {
            resourceDict[resource.resourceType] = resource;
        }
    }

    private void Start()
    {
        // foreach (var resource in caveResourceList)
        // {
        //     var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
        //     resourceCapacityDict[resource.resourceType] = 20;
        //     caveResourceUIDict[resource.resourceType] = newResourceUI;
        //     newResourceUI.Setup(resource.resourceIcon, resourceCapacityDict[resource.resourceType], CaveMaxCapacity);
        // }

        foreach (var pair in resourceCapacityDict)
        {
            var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
            caveResourceUIDict[pair.Key] = newResourceUI;
            newResourceUI.Setup(resourceDict[pair.Key].resourceIcon, pair.Value, CaveMaxCapacity);
        }
    }

    public int AddStorage(EnumPack.ResourceType resourceType, int amount, Action callback)
    {
        int availableStorage;

        if (resourceCapacityDict.ContainsKey(resourceType))
        {
            availableStorage = CaveMaxCapacity - resourceCapacityDict[resourceType];
        }
        else
        {
            availableStorage = CaveMaxCapacity;
            resourceCapacityDict[resourceType] = 0;
        }

        if (amount < availableStorage)
        {
            resourceCapacityDict[resourceType] += amount;

            if (!caveResourceUIDict.ContainsKey(resourceType))
            {
                var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
                newResourceUI.Setup(caveResourceList.FirstOrDefault(a => a.resourceType == resourceType).resourceIcon, resourceCapacityDict[resourceType], CaveMaxCapacity);
                caveResourceUIDict[resourceType] = newResourceUI;
            }
            else
            {
                caveResourceUIDict[resourceType].UpdateCapacity(resourceCapacityDict[resourceType]);
            }

            callback?.Invoke();
            return 0;
        }
        else
        {
            resourceCapacityDict[resourceType] = CaveMaxCapacity;

            if (!caveResourceUIDict.ContainsKey(resourceType))
            {
                var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
                newResourceUI.Setup(caveResourceList.FirstOrDefault(a => a.resourceType == resourceType).resourceIcon, resourceCapacityDict[resourceType], CaveMaxCapacity);
                caveResourceUIDict[resourceType] = newResourceUI;
            }
            else
            {
                caveResourceUIDict[resourceType].UpdateCapacity(resourceCapacityDict[resourceType]);
            }

            callback?.Invoke();
            return amount - availableStorage;
        }
    }

    public EnumPack.ResourceType SuitableResouce()
    {
        var copyList = new List<ResourceConfig>(caveResourceList);

        foreach (var resource in copyList)
        {
            if (resourceCapacityDict[resource.resourceType] == CaveMaxCapacity) copyList.Remove(resource);
        }

        return copyList[Random.Range(0, copyList.Count)].resourceType;
    }

    public bool IsCaveFull()
    {
        return isCaveFull;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICaveMan>(out var caveMan))
        {
            caveMan.TriggerActionCave();
            showableUI.Show(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICaveMan>(out var caveMan))
        {
            caveMan.ExitTriggerActionCave();
            showableUI.Show(false);
        }
    }

    public void UpdateResourceCapacity()
    {
        ResourceCapacityJson = JsonConvert.SerializeObject(resourceCapacityDict);
    }
}

public class CaveResource
{
    public EnumPack.ResourceType resourceType;
    public int resourceCapacity;
}
