using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HMenuController : GameComponent
{
    [SerializeField] private GameObject menuUI;
    
    [Header("BUTTON")]
    [SerializeField] private UIButton settingBtn;

    [Header("POPUP")]
    [SerializeField, PopupPickup] private string settingsPopup;

    [Header("EVENT")] 
    [SerializeField] private PopupShowEvent popupShowEvent;

    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;

    protected override void OnEnabled()
    {
        getPopupParentEvent.OnRaised += getPopupParent_OnRaised;
    }
    
    private GameObject getPopupParent_OnRaised()
    {
        return gameObject;
    }

    protected override void OnDisabled()
    {
        getPopupParentEvent.OnRaised -= getPopupParent_OnRaised;
    }

    private void Start()
    {
        settingBtn.onClick.AddListener(ShowSettingsPopup);
    }

    private void ShowSettingsPopup()
    {
        popupShowEvent.Raise(settingsPopup, transform);
    }

    public void ToggleMenuUI()
    {
        
    }
}
