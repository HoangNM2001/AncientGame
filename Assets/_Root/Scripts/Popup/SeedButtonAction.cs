using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class SeedButtonAction : ButtonAction
{
    [SerializeField, PopupPickup] string chooseSeedPopup;
    [SerializeField] private PopupShowEvent popupShowEvent;
    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;
    
    private Transform popupParentTrans;

    protected override void Initialize()
    {
        base.Initialize();
        popupParentTrans = getPopupParentEvent.Raise().transform;
    }

    protected override void StartAction()
    {
        popupCloseEvent.Raise();
        popupShowEvent.Raise(chooseSeedPopup, popupParentTrans);
    }
}