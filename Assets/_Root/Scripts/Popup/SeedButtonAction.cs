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
    [SerializeField] private ScriptableEventGetGameObject getCurrentExtendFieldEvent;

    private Transform popupParentTrans;
    private ExtendField currentExtendField;

    protected override void Initialize()
    {
        base.Initialize();
        popupParentTrans = getPopupParentEvent.Raise().transform;
    }

    protected override void StartAction()
    {
        currentExtendField = getCurrentExtendFieldEvent.Raise().GetComponent<ExtendField>();

        if (currentExtendField == null) return;

        popupCloseEvent.Raise();
        popupShowEvent.Raise(chooseSeedPopup, popupParentTrans);
    }
}