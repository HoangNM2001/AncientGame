using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using TMPro;
using UnityEngine;

public class Henhouse : SaveDataElement
{
    [SerializeField] private GameObjectPool eggPool;
    [SerializeField] private ResourceConfig eggsResourceConfig;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;
    [SerializeField] private ShowableUI showableUI;
    [SerializeField] private TextMeshProUGUI eggCountText;
    [SerializeField] private Trigger triggerEggsCount;
    [SerializeField] private Chicken chickenPrefab;
    [SerializeField] private Transform chickenParent;

    private List<Chicken> chickenList;
    private const int MAX_EGGS = 20;

    public bool CanSpawnEggs => EggCount < MAX_EGGS;

    public int EggCount
    {
        get => Data.Load(uniqueId + "EggCount", 0);
        set
        {
            eggCountText.SetText($"{value} / {MAX_EGGS}");
            Data.Save(uniqueId + "EggCount", value);
        }
    }

    public int ChickenCount
    {
        get => Data.Load(uniqueId + "ChickenCount", 3);
        set => Data.Save(uniqueId + "ChickenCount", value);
    }

    private void Awake()
    {
        chickenList = new List<Chicken>();

        for (var i = 0; i < ChickenCount; i++)
        {
            var chicken = Instantiate(chickenPrefab, chickenParent);
            chicken.Setup(this);
            chickenList.Add(chicken);
        }

        for (var i = 0; i < EggCount; i++)
        {
            var egg = eggPool.Request();
            egg.GetComponent<Egg>().Setup(this);
            egg.transform.position = transform.position + GetRandomPosition(3f);
        }

        eggCountText.SetText($"{EggCount} / {MAX_EGGS}");
    }

    public void SpawnEgg(Vector3 spawnPos)
    {
        var egg = eggPool.Request();
        egg.transform.position = spawnPos;
        egg.transform.DOJump(egg.transform.position + GetRandomPosition(0.5f), 2, 1, 0.5f).SetEase(Ease.InSine);
        egg.GetComponent<Egg>().Setup(this);

        EggCount++;
    }

    public void HarvestEgg(GameObject egg)
    {
        eggPool.Return(egg);
        flyUIEvent.Raise(new FlyEventData
        {
            resourceType = eggsResourceConfig.resourceType,
            worldPos = egg.transform.position
        });

        EggCount--;
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

    private Vector3 GetRandomPosition(float radius)
    {
        return SimpleMath.RandomVector3(true) * radius;
    }

    private void OnApplicationQuit()
    {

    }
}
