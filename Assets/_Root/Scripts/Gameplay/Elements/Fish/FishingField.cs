using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using TMPro;
using UnityEngine;

public class FishingField : SaveDataElement
{
    [SerializeField] private Tile tile;
    [SerializeField] private FishingData fishingData;
    [SerializeField] private Trigger triggerAround;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;
    [SerializeField] private PlayerLevel playerLevel;

    private List<Fish> _fishList = new();
    private List<Fish> _fishInstanceList = new();

    public Tile Tile => tile;

    private void Awake()
    {
        Initialize();
    }

    private void Start()
    {
        _fishList = fishingData.FishList;
    }

    public override void Activate(bool restore = true)
    {
        DOTween.Kill(transform);
        gameObject.SetActive(true);

        if (restore)
        {
            transform.localScale = DefaultScale;
            OnActivated();
        }
        else
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(DefaultScale, AnimDuration).SetEase(Ease.OutBack).SetTarget(transform).OnComplete(OnActivated);
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        OnDeactivated();
    }

    // private void Activate()
    // {
    //     _fishInstanceList = new List<Fish>(_fishList.Count);
    //
    //     foreach (var fish in _fishList)
    //     {
    //         var newFish = Instantiate(fish, transform);
    //         newFish.SetView();
    //         _fishInstanceList.Add(newFish);
    //     }
    //
    //     gameObject.SetActive(true);
    //     DOTween.Kill(transform);
    //     OnActivated();
    // }

    public void HarvestFish()
    {
        foreach (var fish in fishingData.caughtList)
        {
            var tempFly = fish.flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = Vector3.zero;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                playerLevel.AddExp(fish.exp);
                ShowFlyText(transform.position, $"+ {playerLevel.ExpUp} Exp");
                fish.flyModelPool.Return(tempFly);
                flyUIEvent.Raise(new FlyEventData
                {
                    resourceType = fish.resourceType,
                    worldPos = tempFly.transform.position
                });
            });
            fish.resourceQuantity.Value++;
        }

        fishingData.caughtList.Clear();
    }

    private void OnActivated()
    {
        _fishInstanceList = new List<Fish>(_fishList.Count);

        foreach (var fish in _fishList)
        {
            var newFish = Instantiate(fish, transform);
            newFish.SetView();
            _fishInstanceList.Add(newFish);
        }
        
        triggerAround.EnterTriggerEvent += OnTriggerAroundEnterEvent;
        triggerAround.ExitTriggerEvent += OnTriggerAroundExitEvent;
    }

    private void OnDeactivated()
    {
        triggerAround.EnterTriggerEvent -= OnTriggerAroundEnterEvent;
        triggerAround.ExitTriggerEvent -= OnTriggerAroundExitEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IFisher>(out var fisher)) fisher.TriggerActionFishing(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IFisher>(out var fisher)) fisher.ExitTriggerAction();
    }

    private void OnTriggerAroundEnterEvent(Collider collider)
    {
        if (collider.TryGetComponent<IFisher>(out var fisher))
        {
            foreach (var fish in _fishInstanceList)
            {
                fish.MoveAround();
            }
        }
    }

    private void OnTriggerAroundExitEvent(Collider collider)
    {
        if (collider.TryGetComponent<IFisher>(out var fisher))
        {
            foreach (var fish in _fishInstanceList)
            {
                fish.StopMoveAround();
            }
        }
    }
}
