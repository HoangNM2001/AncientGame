using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Pancake;
using Pancake.Apex;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEngine;

public class FarmAction : GameComponent, ICharacterAction
{
    [SerializeField] private EnumPack.CharacterActionType characterActionType;
    [SerializeField] private FarmTool farmTool;
    [SerializeField] private CharacterAnimController characterAnimController;
    [SerializeField] private bool isPlayer;
    [ShowIf(nameof(isPlayer)), SerializeField] private PlayerStat playerStat;
    [HideIf(nameof(isPlayer)), SerializeField] private CharacterStat characterStat;

    private bool _activated;
    private string _actionAnimName;

    public FarmTool FarmTool => farmTool;
    public EnumPack.CharacterActionType CharacterActionType => characterActionType;
    public bool Activated => _activated;

    private void Awake()
    {
        _activated = false;
    }

    private void Start()
    {
        _actionAnimName = characterActionType.ToString();

        farmTool.Initialize(characterActionType, isPlayer);
    }

    public void Activate()
    {
        if (_activated) return;
        _activated = true;

        int toolIndex;
        float workSpeed;

        if (isPlayer)
        {
            toolIndex = playerStat.ToolIndex;
            workSpeed = playerStat.WorkSpeed;
        }
        else
        {
            toolIndex = 1;
            workSpeed = characterStat.WorkSpeed;
        }

        if (characterAnimController)
        {
            characterAnimController.Speed = workSpeed;
        }

        farmTool.Activate(toolIndex);
        PlayAnimation();
    }

    public void Deactivate()
    {
        if (!_activated) return;
        _activated = false;

        if (characterAnimController)
        {
            characterAnimController.Speed = 1;
        }

        farmTool.Deactivate();
        StopAnimation();
    }

    private void PlayAnimation()
    {
        characterAnimController.Play(_actionAnimName, 1);
    }

    private void StopAnimation()
    {
        characterAnimController.Play(Constant.EMPTY, 1);
        characterAnimController.Play(Constant.IDLE_2_RUN, 0);
    }
}