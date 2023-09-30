using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class CharacterHandleTrigger : GameComponent
{
    [Header("INTERACT")]
    [SerializeField, PopupPickup] private string farmActionPopup;
    [SerializeField] private PopupShowEvent popupShowEvent;
    [SerializeField] private ScriptableEventNoParam popupCloseEvent;
    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;
    [SerializeField] private ScriptableEventGetGameObject getCurrentExtendFieldEvent;
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private ScriptableEventNoParam stopActionEvent;

    private Transform popupParentTrans;
    private GameObject currentExtendField;

    protected override void OnEnabled()
    {
        getCurrentExtendFieldEvent.OnRaised += GetCurrentExtendFieldEvent_OnRaise;
    }

    protected override void OnDisabled()
    {
        getCurrentExtendFieldEvent.OnRaised -= GetCurrentExtendFieldEvent_OnRaise;
    }

    private GameObject GetCurrentExtendFieldEvent_OnRaise()
    {
        return currentExtendField;
    }

    private void Start()
    {
        popupParentTrans = getPopupParentEvent.Raise().transform;
    }

    public void TriggerActionFarm(GameObject triggerField)
    {
        currentExtendField = triggerField;
        popupShowEvent.Raise(farmActionPopup, popupParentTrans);
    }

    public void ExitTriggerActionFarm()
    {
        currentExtendField = null;
        popupCloseEvent.Raise();
        stopActionEvent.Raise();
    }
}
