using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pancake;
using Pancake.Scriptable;
using Pancake.Threading.Tasks.Triggers;
using UnityEngine;

public class HuntingField : SaveDataElement
{
    [SerializeField] private MapPredator predator;
    [SerializeField] private float respawnInterval;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    public MapPredator Predator => predator;

    private void Start()
    {
        Activate();
    }

    private void Activate()
    {
        predator.gameObject.SetActive(true);
        predator.Activate(transform.position);
    }

    private void Deactivate()
    {
        predator.gameObject.SetActive(false);
    }

    public void HarvestOnWin()
    {
        for (var i = 0; i < predator.NumberOfMeat; i++)
        {
            var tempFly = predator.MeatResource.flyModelPool.Request();
            tempFly.transform.SetParent(transform);
            tempFly.transform.localPosition = predator.transform.localPosition;
            tempFly.GetComponent<ResourceFlyModel>().DoBouncing(() =>
            {
                predator.MeatResource.flyModelPool.Return(tempFly);
                flyUIEvent.Raise(new FlyEventData
                {
                    resourceType = predator.MeatResource.resourceType,
                    worldPos = tempFly.transform.position
                });
            });
        }
        
        predator.DropMeat();
        Deactivate();
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHunter>(out var hunter))
        {
            hunter.TriggerActionHunting(gameObject);
            predator.PlayerInSight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IHunter>(out var hunter))
        {
            hunter.ExitTriggerAction();
            predator.PlayerInSight(false);
        }
    }
}
