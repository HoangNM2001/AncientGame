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
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private ScriptableEventNoParam stopActionEvent;

    private Transform popupParentTrans;

    private void Start()
    {
        popupParentTrans = getPopupParentEvent.Raise().transform;
    }

    public void TriggerActionFarm()
    {
        popupShowEvent.Raise(farmActionPopup, popupParentTrans);
    }

    public void ExitTriggerActionFarm()
    {
        popupCloseEvent.Raise();
        stopActionEvent.Raise();
    }
}
