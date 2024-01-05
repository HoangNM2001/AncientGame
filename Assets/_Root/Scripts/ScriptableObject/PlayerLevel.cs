using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Player/PlayerLevel")]
public class PlayerLevel : ScriptableObject
{
    [SerializeField] private int maxLevel = 100;
    [SerializeField] private float expFactor0 = 140;
    [SerializeField] private float expFactor1 = 1.5f;

    public bool IsMaxLevel => Level >= maxLevel;
    public double RequiredExp => IsMaxLevel ? 0 : expFactor0 * Mathf.Pow(expFactor1, Level + 1);
    public bool CanLevelUp => !IsMaxLevel && Exp >= RequiredExp;

    public Action LevelChangedEvent;

    public int Level
    {
        get => Data.Load("level", 1);
        private set => Data.Save("level", value);
    }
    public double Exp
    {
        get => Data.Load("exp", 0);
        private set => Data.Save("exp", value);
    }
    public int SkillPoint
    {
        get => Data.Load("skillpoint", 0);
        private set => Data.Save("skillpoint", value);
    }

    
}

