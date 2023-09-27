using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEngine;

public class FarmAction : GameComponent, ICharacterAction
{
    [SerializeField] private EnumPack.CharacterActionType characterActionType;
    [SerializeField] private FarmTool farmTool;

    [Header("EVENT")] [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;

    private bool activated;
    private string actionAnimName;
    private CharacterController characterController;
    private CharacterAnimController characterAnimController;

    public EnumPack.CharacterActionType CharacterActionType => characterActionType;
    public bool Activated => activated;

    private void Awake()
    {
        activated = false;
    }

    private void Start()
    {
        characterController = getCharacterEvent.Raise().gameObject.GetComponent<CharacterController>();
        characterAnimController = characterController.CharacterAnimController;

        actionAnimName = characterActionType.ToString();

        farmTool.Initialize(characterActionType);
    }

    public void Activate()
    {
        if (activated) return;
        activated = true;

        if (characterAnimController)
        {
            characterAnimController.Speed = characterController.CharacterStat.workingSpeed;
        }
        
        farmTool.Activate(1);
        PlayAnimation();
    }

    public void Deactivate()
    {
        if (!activated) return;
        activated = false;

        if (characterAnimController)
        {
            characterAnimController.Speed = 1;
        }
        
        farmTool.Deactivate();
        StopAnimation();
    }

    private void PlayAnimation()
    {
        characterAnimController.Play(actionAnimName, 1);
    }

    private void StopAnimation()
    {
        characterAnimController.Play(Constant.EMPTY, 1);
        characterAnimController.Play(Constant.IDLE_2_RUN, 0);
    }
}