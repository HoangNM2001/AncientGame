using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class CharacterHandleTrigger : GameComponent, IFarmer, ICaveMan, IFisher, IHunter
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
    [SerializeField, PopupPickup] private string fishingActionPopup;
    [SerializeField, PopupPickup] private string huntingActionPopup;
    [SerializeField, PopupPickup] private string saveSlaveActionPopup;
    [SerializeField, PopupPickup] private string caveActionPopup;

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

    public void TriggerActionShopNear(GameObject triggerShop)
    {
        currentInteract = triggerShop;
        popupShowEvent.Raise(shopActionPopup, popupParentTrans);
    }

    public void TriggerActionCave(GameObject triggerCave)
    {
        currentInteract = triggerCave;
        popupShowEvent.Raise(caveActionPopup, popupParentTrans);
    }

    public void TriggerActionFishing(GameObject fishingField)
    {
        currentInteract = fishingField;
        popupShowEvent.Raise(fishingActionPopup, popupParentTrans);
    }

    public void TriggerActionHunting(GameObject predator)
    {
        currentInteract = predator;
        popupShowEvent.Raise(huntingActionPopup, popupParentTrans);
    }

    public void TriggerSaveSlave(GameObject drownSlave)
    {
        currentInteract = drownSlave;
        popupShowEvent.Raise(saveSlaveActionPopup, popupParentTrans);
    }

    public void ExitTriggerAction()
    {
        currentInteract = null;
        popupCloseEvent.Raise();
    }
}
