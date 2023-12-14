using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class MiniGameController : GameComponent
{
    [SerializeField] private IntVariable currentMiniGameType;
    [SerializeField] private ScriptableEventBool toggleMiniGame;

    private List<IMiniGame> minigameList = new List<IMiniGame>();

    protected override void OnEnabled()
    {
        toggleMiniGame.OnRaised += toggleMiniGame_OnRaised;
    }

    private void toggleMiniGame_OnRaised(bool active)
    {
        if (active)
        {
            minigameList.FirstOrDefault(a => (int)a.MiniGameType == currentMiniGameType.Value)?.Activate();
        }
        else
        {
            minigameList.FirstOrDefault(a => (int)a.MiniGameType == currentMiniGameType.Value)?.Deactivate();
        }

    }

    protected override void OnDisabled()
    {
        toggleMiniGame.OnRaised -= toggleMiniGame_OnRaised;
    }

    private void Start()
    {
        minigameList = GetComponentsInChildren<IMiniGame>().ToList();
        foreach (var minigame in minigameList)
        {
            minigame.Deactivate();
        }
    }
}

[Serializable]
public class LeftRightCouple
{
    public Transform leftTrans;
    public Transform rightTrans;
}
