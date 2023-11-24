using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using UnityEngine;

public class PlayerAnimController : CharacterAnimController
{
    [Header("EVENT")]
    [SerializeField] private ScriptableEventNoParam harvestFruitEvent;

    public void TriggerHarvestFruit()
    {
        harvestFruitEvent.Raise();
    }
}
