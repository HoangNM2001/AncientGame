using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using UnityEngine;

public class FruitTree : GameComponent
{
    [SerializeField] private ResourceConfig fruitResource;
    [SerializeField] private Transform standPosition;
    [SerializeField] private List<Fruit> fruitList;

    public ResourceConfig FruitResource => fruitResource;
    public Vector3 StandPosition => standPosition.position;

    private void OnTriggerEnter(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.TriggerActionTree(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(other);
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerActionTree();
    }

#if UNITY_EDITOR
    [ContextMenu("Get All Fruits")]
    public void GetAllFruits()
    {
        fruitList = GetComponentsInChildren<Fruit>().ToList();
    }
#endif
}