using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Henhouse : SaveDataElement
{
    [SerializeField] private GameObjectPool eggsPool;
    [SerializeField] private ShowableUI showableUI;
    [SerializeField] private Trigger triggerEggsCount;
    [SerializeField] private Chicken chickenPrefab;
    [SerializeField] private Transform chickenParent;
    
    private List<Chicken> chickenList;
    private const int MAX_EGGS = 20;

    public bool IsCanSpawnEggs => EggCount < MAX_EGGS;

    public int EggCount
    {
        get => Data.Load(uniqueId + "EggCount", 0);
        set => Data.Save(uniqueId + "EggCount", value);
    }

    public int ChickenCount
    {
        get => Data.Load(uniqueId + "ChickenCount", 3);
        set => Data.Save(uniqueId + "ChickenCount", value);
    }

    private void Awake()
    {
        for (var i = 0; i < ChickenCount; i++)
        {
            var chicken = Instantiate(chickenPrefab, chickenParent);
            chicken.Setup(this);
            chickenList.Add(chicken);
        }
    }

    protected override void OnEnabled()
    {
        triggerEggsCount.EnterTriggerEvent += TriggerEggsCount;
        triggerEggsCount.ExitTriggerEvent += ExitTriggerEggsCount;
    }

    protected override void OnDisabled()
    {
        triggerEggsCount.EnterTriggerEvent -= TriggerEggsCount;
        triggerEggsCount.ExitTriggerEvent -= ExitTriggerEggsCount;
    }

    private void TriggerEggsCount(Collider collider)
    {
        showableUI.Show(true);
    }

    private void ExitTriggerEggsCount(Collider collider)
    {
        showableUI.Show(false);
    }
}
