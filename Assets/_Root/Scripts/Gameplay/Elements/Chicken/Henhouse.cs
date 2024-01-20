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
    [SerializeField] private PlayerLevel playerLevel;
    [SerializeField] private ResourceConfig eggsResourceConfig;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;
    [SerializeField] private ShowableUI showableUI;
    [SerializeField] private TextMeshProUGUI chickenCountText;
    [SerializeField] private TextMeshProUGUI eggCountText;
    [SerializeField] private Trigger triggerEggsCount;
    [SerializeField] private Trigger triggerPopup;
    [SerializeField] private Chicken chickenPrefab;
    [SerializeField] private Transform chickenParent;

    private List<Chicken> chickenList;
    private List<Egg> eggList;
    private const int MAX_EGGS = 20;
    private const int MAX_CHICKENS = 10;

    public bool CanSpawnEggs => EggCount < MAX_EGGS;
    private bool CanSpawnChickens => ChickenCount < MAX_CHICKENS;

    private int EggCount
    {
        get => Data.Load(Id + "EggCount", 0);
        set
        {
            if (value < 0) value = 0;
            eggCountText.SetText($"{value} / {MAX_EGGS}");
            Data.Save(Id + "EggCount", value);
        }
    }

    private int ChickenCount
    {
        get => Data.Load(Id + "ChickenCount", 3);
        set
        {
            chickenCountText.SetText($"{value} / {MAX_CHICKENS}");
            Data.Save(Id + "ChickenCount", value);
        }
    }

    private void Awake()
    {
        Initialize();

        chickenList = new List<Chicken>();
        eggList = new List<Egg>();

        for (var i = 0; i < ChickenCount; i++)
        {
            var chicken = Instantiate(chickenPrefab, chickenParent);
            chicken.Setup(this);
            chickenList.Add(chicken);
        }

        for (var i = 0; i < EggCount; i++)
        {
            var egg = eggPool.Request().GetComponent<Egg>();
            egg.Setup(this);
            egg.transform.position = transform.position + GetRandomPosition(3f);
            eggList.Add(egg);
        }

        chickenCountText.SetText($"{ChickenCount} / {MAX_CHICKENS}");
        eggCountText.SetText($"{EggCount} / {MAX_EGGS}");
    }

    public void SpawnChicken()
    {
        if (!CanSpawnChickens) return;

        var chicken = Instantiate(chickenPrefab, chickenParent);
        chicken.Setup(this);
        chickenList.Add(chicken);

        ChickenCount++;
    }

    public void SpawnEgg(Vector3 spawnPos)
    {
        var egg = eggPool.Request().GetComponent<Egg>();
        egg.transform.position = spawnPos;
        egg.transform.DOJump(egg.transform.position + GetRandomPosition(0.5f), 2, 1, 0.5f).SetEase(Ease.InSine);
        egg.Setup(this);

        eggList.Add(egg);
        EggCount++;
    }

    public void HarvestAllEggs()
    {
        if (eggList.IsNullOrEmpty()) return;

        foreach (var egg in eggList)
        {
            EggFly(egg);
        }

        eggList.Clear();
    }

    public void HarvestEgg(Egg egg)
    {
        EggFly(egg);
        eggList.Remove(egg);
    }

    private void EggFly(Egg egg)
    {
        eggPool.Return(egg.gameObject);
        flyUIEvent.Raise(new FlyEventData
        {
            resourceType = eggsResourceConfig.resourceType,
            worldPos = egg.transform.position
        });
        eggsResourceConfig.resourceQuantity.Value++;

        EggCount--;

        playerLevel.AddExp(eggsResourceConfig.exp);
        ShowFlyText(egg.transform.position, $"+ {playerLevel.ExpUp} Exp");
    }

    protected override void OnEnabled()
    {
        triggerEggsCount.EnterTriggerEvent += TriggerEggsCount;
        triggerEggsCount.ExitTriggerEvent += ExitTriggerEggsCount;
        triggerPopup.EnterTriggerEvent += TriggerPopup;
        triggerPopup.ExitTriggerEvent += ExitTriggerPopup;
    }

    protected override void OnDisabled()
    {
        triggerEggsCount.EnterTriggerEvent -= TriggerEggsCount;
        triggerEggsCount.ExitTriggerEvent -= ExitTriggerEggsCount;
        triggerPopup.EnterTriggerEvent -= TriggerPopup;
        triggerPopup.ExitTriggerEvent -= ExitTriggerPopup;
    }

    private void TriggerPopup(Collider collider)
    {
        var characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(collider);
        if (characterHandleTrigger) characterHandleTrigger.TriggerHenHouse(gameObject);
    }

    private void ExitTriggerPopup(Collider collider)
    {
        var characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(collider);
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerAction();
    }

    private void TriggerEggsCount(Collider other)
    {
        showableUI.Show(true);
    }

    private void ExitTriggerEggsCount(Collider other)
    {
        showableUI.Show(false);
    }

    private Vector3 GetRandomPosition(float radius)
    {
        return SimpleMath.RandomVector3(true) * radius;
    }
}
