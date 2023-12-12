using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using TMPro;
using UnityEngine;

public class FishingField : GameComponent
{
    [SerializeField] private FishingData fishingData;
    [SerializeField] private Trigger triggerAround;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    private List<Fish> fishList = new List<Fish>();
    private List<Fish> fishInstanceList = new List<Fish>();

    private void Start()
    {
        fishList = fishingData.FishList;

        Activate();
    }

    private void Activate()
    {
        fishInstanceList = new List<Fish>(fishList.Count);

        foreach (var fish in fishList)
        {
            var newFish = Instantiate(fish, transform);
            newFish.SetView();
            fishInstanceList.Add(newFish);
        }

        gameObject.SetActive(true);
        DOTween.Kill(transform);
        OnActivated();
    }

    public void HarvestFish()
    {
        foreach (var fish in fishingData.caughtList)
        {
            var tempFly = fish.flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = Vector3.zero;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                fish.flyModelPool.Return(tempFly);
                flyUIEvent.Raise(new FlyEventData
                {
                    resourceType = fish.resourceType,
                    worldPos = tempFly.transform.position
                });
            });
        }

        fishingData.caughtList.Clear();
    }

    private void OnActivated()
    {
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
            foreach (var fish in fishInstanceList)
            {
                fish.MoveAround();
            }
        }
    }

    private void OnTriggerAroundExitEvent(Collider collider)
    {
        if (collider.TryGetComponent<IFisher>(out var fisher))
        {
            foreach (var fish in fishInstanceList)
            {
                fish.StopMoveAround();
            }
        }
    }
}
