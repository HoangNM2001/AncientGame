using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class ButtonAction : GameComponent
{
    [SerializeField] private EnumPack.CharacterActionType characterActionType;
    [SerializeField] private ScriptableEventInt startActionEvent;
    [SerializeField] private ScriptableEventNoParam popupCloseEvent;
    
    private UIButton actionButton;

    private void Awake()
    {
        actionButton = GetComponent<UIButton>();
    }

    private void Start()
    {
        actionButton.onClick.AddListener(StartAction);
    }

    private void StartAction()
    {
        startActionEvent.Raise((int)characterActionType);
        popupCloseEvent.Raise();
    }
}
