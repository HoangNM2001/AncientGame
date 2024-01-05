using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Trigger : GameComponent
{
    public Action<Collider> EnterTriggerEvent { get; set; }
    public Action<Collider> ExitTriggerEvent { get; set; }

    public GameObject parent;

    private Rigidbody thisRigidbody;
    private Collider thisCollider;

    private void Awake()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        EnterTriggerEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ExitTriggerEvent?.Invoke(other);
    }

    public List<GameObject> GetObjectNear()
    {
        var colliders = Physics.OverlapBox(thisCollider.bounds.center, thisCollider.bounds.extents / 2);

        List<GameObject> nearObjects = new List<GameObject>();

        foreach (var col in colliders)
        {
            if (col.gameObject != parent && col.gameObject != gameObject && col.gameObject.activeInHierarchy) 
            {
                nearObjects.Add(col.gameObject);
            }
        }

        return nearObjects;
    }
}