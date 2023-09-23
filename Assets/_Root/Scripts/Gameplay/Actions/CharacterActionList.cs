using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEngine;

public class CharacterActionList : GameComponent
{
    [SerializeField] private ScriptableEventInt startActionEvent;
    [SerializeField] private ScriptableEventNoParam stopActionEvent;

    private List<ICharacterAction> characterActionList;
    private ICharacterAction currentCharacterAction;

    public ICharacterAction CurrentCharacterAction => currentCharacterAction;

    private void Awake()
    {
        characterActionList = GetComponentsInChildren<ICharacterAction>().ToList();

        currentCharacterAction = null;
    }

    protected override void OnEnabled()
    {
        startActionEvent.OnRaised += StartActionEvent;
        stopActionEvent.OnRaised += StopActionEvent;
    }

    protected override void OnDisabled()
    {
        startActionEvent.OnRaised -= StartActionEvent;
        stopActionEvent.OnRaised -= StopActionEvent;
    }

    private void StartActionEvent(int actionTypeInt)
    {
        if (currentCharacterAction is { Activated: true })
        {
            currentCharacterAction.Deactivate();
        }
        
        currentCharacterAction =
            characterActionList.FirstOrDefault(a => a.CharacterActionType == (CharacterActionType)actionTypeInt);
        
        if (currentCharacterAction is { Activated: false })
        {
            currentCharacterAction.Activate();
        }
    }

    private void StopActionEvent()
    {
        foreach (var action in characterActionList.Where(action => action.Activated))
        {
            action.Deactivate();
        }

        currentCharacterAction = null;
    }
}