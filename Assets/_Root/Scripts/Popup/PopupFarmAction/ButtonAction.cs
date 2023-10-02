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
    [SerializeField] protected EnumPack.CharacterActionType characterActionType;
    [SerializeField] protected ScriptableEventInt startActionEvent;
    [SerializeField] protected ScriptableEventNoParam popupCloseEvent;
    
    private UIButton actionButton;

    private void Awake()
    {
        actionButton = GetComponent<UIButton>();
    }

    private void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        actionButton.onClick.AddListener(StartAction);
    }

    protected virtual void StartAction()
    {
        startActionEvent.Raise((int)characterActionType);
        popupCloseEvent.Raise();
    }
}
