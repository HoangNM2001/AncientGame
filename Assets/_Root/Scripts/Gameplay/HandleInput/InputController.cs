using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class InputController : GameComponent
{
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private List<UIInput> uiInputList;

    protected override void OnEnabled()
    {
        changeInputEvent.OnRaised += changeInputEvent_OnRaised;
    }
    
    private void changeInputEvent_OnRaised(int controlType)
    {
        foreach (var uiInput in uiInputList)
        {
            uiInput.gameObject.SetActive((int)uiInput.ControlType == controlType);
        }
    }

    protected override void OnDisabled()
    {
        changeInputEvent.OnRaised -= changeInputEvent_OnRaised;
    }

    private void Start()
    {
        foreach (var uiInput in uiInputList)
        {
            uiInput.gameObject.SetActive(uiInput.ControlType == EnumPack.ControlType.Move);
        }
    }
}
