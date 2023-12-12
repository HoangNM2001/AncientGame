using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pancake;
using Pancake.Scriptable;
using Pancake.Threading.Tasks.Triggers;
using UnityEngine;

public class HuntingField : GameComponent
{
    [SerializeField] private MapPredator predator;
    [SerializeField] private float respawnInterval;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

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
            tempFly.transform.localPosition = Vector3.zero;
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
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IHunter>(out var hunter))
        {
            hunter.TriggerActionHunting(predator.gameObject);
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
