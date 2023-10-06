using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class Trigger : GameComponent
{
    public Action<Collider> EnterTriggerEvent { get; set; }
    public Action<Collider> ExitTriggerEvent { get; set; }

    private void OnTriggerEnter(Collider other)
    {
        EnterTriggerEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        ExitTriggerEvent?.Invoke(other);
    }
}