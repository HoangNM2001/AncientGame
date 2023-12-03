using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Pancake;
using Pancake.Threading.Tasks.Triggers;
using UnityEngine;

public class HuntingField : GameComponent
{
    [SerializeField] private MapPredator predator;
    [SerializeField] private float respawnInterval;

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
            hunter.ExitTriggerActionHunting();
            predator.PlayerInSight(false);
        }
    }
}
