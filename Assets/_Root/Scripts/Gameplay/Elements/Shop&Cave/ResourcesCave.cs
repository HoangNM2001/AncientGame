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
    [SerializeField] private Transform resourceUIParent;
    [SerializeField] private Transform goToPos;

    private const int MaxNumberOfResources = 3;
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

    public bool IsCaveAvailable => resourceCapacityDict.Count < MaxNumberOfResources ||
                                   resourceCapacityDict.Any(pair => pair.Value < CaveMaxCapacity);

    public Transform GoToPos => goToPos;
    public List<ResourceConfig> CaveResourceList => caveResourceList;

    private void Awake()
    {
        caveResourceUIDict = new Dictionary<EnumPack.ResourceType, CaveResourcesUI>();
        resourceCapacityDict =
            JsonConvert.DeserializeObject<Dictionary<EnumPack.ResourceType, int>>(ResourceCapacityJson) ??
            new Dictionary<EnumPack.ResourceType, int>();
        resourceDict = new Dictionary<EnumPack.ResourceType, ResourceConfig>();

        foreach (var resource in caveResourceList)
        {
            resourceDict[resource.resourceType] = resource;
        }
    }

    private void Start()
    {
        foreach (var pair in resourceCapacityDict)
        {
            var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
            caveResourceUIDict[pair.Key] = newResourceUI;
            newResourceUI.Setup(resourceDict[pair.Key].resourceIcon, CaveMaxCapacity);
            newResourceUI.UpdateCapacity(pair.Value);
        }
    }

    public bool IsAddable(EnumPack.ResourceType resourceType)
    {
        if (resourceCapacityDict.Count < MaxNumberOfResources) return true;

        if (resourceCapacityDict.TryGetValue(resourceType, out var value)) return value < CaveMaxCapacity;

        return false;
    }

    public int AddStorage(EnumPack.ResourceType resourceType, int amount, Action callback)
    {
        int availableStorage;
        int slaveRemainCapacity;

        if (resourceCapacityDict.TryGetValue(resourceType, out var currentCapacity))
        {
            availableStorage = CaveMaxCapacity - currentCapacity;
        }
        else
        {
            availableStorage = CaveMaxCapacity;
            UpdateResourceCapacity(resourceType, 0);
        }

        if (amount < availableStorage)
        {
            UpdateResourceCapacity(resourceType, resourceCapacityDict[resourceType] + amount);
            slaveRemainCapacity = 0;
        }
        else
        {
            UpdateResourceCapacity(resourceType, CaveMaxCapacity);
            slaveRemainCapacity = amount - availableStorage;
        }

        if (!caveResourceUIDict.ContainsKey(resourceType))
        {
            var newResourceUI = Instantiate(caveResourcesUIPrefab, resourceUIParent);
            newResourceUI.Setup(resourceDict[resourceType].resourceIcon, CaveMaxCapacity);
            caveResourceUIDict[resourceType] = newResourceUI;
        }

        Debug.LogError(amount + " - " + availableStorage + " - " + slaveRemainCapacity);
        caveResourceUIDict[resourceType].UpdateCapacity(resourceCapacityDict[resourceType], callback);
        return slaveRemainCapacity;
    }

    public EnumPack.ResourceType SuitableResource()
    {
        if (resourceCapacityDict.Count < MaxNumberOfResources)
        {
            var tempList = caveResourceList.Select(resource => resource.resourceType).ToList();

            foreach (var pair in resourceCapacityDict)
            {
                tempList.Remove(pair.Key);
            }

            return tempList[Random.Range(0, tempList.Count)];
        }
        else
        {
            var tempList =
                (from pair in resourceCapacityDict where pair.Value < CaveMaxCapacity select pair.Key).ToList();

            return tempList.IsNullOrEmpty()
                ? caveResourceList[Random.Range(0, caveResourceList.Count)].resourceType
                : tempList[Random.Range(0, tempList.Count)];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICaveMan>(out var caveMan))
        {
            showableUI.Show(true);
            caveMan.TriggerActionCave();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ICaveMan>(out var caveMan))
        {
            caveMan.ExitTriggerAction();
            showableUI.Show(false);
        }
    }

    private void UpdateResourceCapacity(EnumPack.ResourceType resourceType, int value)
    {
        resourceCapacityDict[resourceType] = value;
        ResourceCapacityJson = JsonConvert.SerializeObject(resourceCapacityDict);
    }
}

public class CaveResource
{
    public EnumPack.ResourceType resourceType;
    public int resourceCapacity;
}