using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class CharacterHandleTrigger : GameComponent
{
    [SerializeField] private PopupShowEvent popupShowEvent;
    [SerializeField] private ScriptableEventNoParam popupCloseEvent;
    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;
    [SerializeField] private ScriptableEventGetGameObject getCurrentInteractEvent;
    
    [Header("Interact Popup")]
    [SerializeField] private ScriptableEventNoParam stopActionEvent;
    [SerializeField, PopupPickup] private string farmActionPopup;
    [SerializeField, PopupPickup] private string fruitActionPopup;
    [SerializeField, PopupPickup] private string shopActionPopup; 

    private Transform popupParentTrans;
    private GameObject currentInteract;

    protected override void OnEnabled()
    {
        getCurrentInteractEvent.OnRaised += getCurrentInteractEvent_OnRaise;
    }

    protected override void OnDisabled()
    {
        getCurrentInteractEvent.OnRaised -= getCurrentInteractEvent_OnRaise;
    }

    private GameObject getCurrentInteractEvent_OnRaise()
    {
        return currentInteract;
    }

    private void Start()
    {
        popupParentTrans = getPopupParentEvent.Raise().transform;
    }

    public void TriggerActionFarm(GameObject triggerField)
    {
        currentInteract = triggerField;
        popupShowEvent.Raise(farmActionPopup, popupParentTrans);
    }

    public void ExitTriggerActionFarm()
    {
        currentInteract = null;
        popupCloseEvent.Raise();
        stopActionEvent.Raise();
    }

    public void TriggerActionTree(GameObject triggerTree)
    {
        currentInteract = triggerTree;
        popupShowEvent.Raise(fruitActionPopup, popupParentTrans);
    }

    public void ExitTriggerActionTree()
    {
        currentInteract = null;
        popupCloseEvent.Raise();
    }

    public void TriggerActionShopFar(GameObject triggerShop)
    {
        Debug.LogError("Far");
    }

    public void ExitTriggerActionShopFar(GameObject triggerShop)
    {
        Debug.LogError("ExitFar");
    }

    public void TriggerActionShopNear(GameObject triggerShop)
    {
        Debug.LogError("Near");
        currentInteract = triggerShop;
        popupShowEvent.Raise(shopActionPopup, popupParentTrans);
    }

    public void ExitTriggerActionShopNear(GameObject triggerShop)
    {
        Debug.LogError("ExitNear");
        currentInteract = null;
        popupCloseEvent.Raise();
    }
}
