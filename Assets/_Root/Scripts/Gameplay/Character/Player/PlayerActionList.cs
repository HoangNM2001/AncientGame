using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerActionList : CharacterActionList
{
    [SerializeField] private ScriptableEventInt startActionEvent;
    [SerializeField] private ScriptableEventNoParam stopActionEvent;

    protected override void OnEnabled()
    {
        startActionEvent.OnRaised += StartActionEvent;
        stopActionEvent.OnRaised += StopActionEvent;
    }

    protected override void OnDisabled()
    {
        startActionEvent.OnRaised -= StartActionEvent;
        stopActionEvent.OnRaised -= StopActionEvent;
    }
}
