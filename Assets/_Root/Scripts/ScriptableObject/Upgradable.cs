using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Apex;
using Pancake.Scriptable;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgradable")]
public class Upgradable : ScriptableObject
{
    [UniqueID] public string uniqueId;
    public IntVariable coinQuantity;
    public PlayerLevel playerLevel;
    public string desc;
    public bool descMultiline;
    public int maxLevel;
    public EnumPack.CostType costType;
    public float valFactor0 = 0.9f;
    public float valFactor1 = 0.1f;
    public float costFactor0 = 1000.0f;
    public float costFactor1 = 1.1f;

    public Action<Upgradable> OnUpgrade;

    public float Value => valFactor0 + valFactor1 * Level;
    public bool IsMaxLevel => Level >= maxLevel;
    public bool IsUpgradable
    {
        get
        {
            if (IsMaxLevel) return false;
            switch (costType)
            {
                case EnumPack.CostType.Coin:
                    return coinQuantity.Value >= Cost;
                case EnumPack.CostType.SkillPoint:
                    return playerLevel.SkillPoint > 0;
                default:
                    return false;
            }
        }
    }

    public int Cost
    {
        get
        {
            switch (costType)
            {
                case EnumPack.CostType.Coin:
                    return (costFactor0 * Mathf.Pow(costFactor1, Level)).RoundToInt();
                case EnumPack.CostType.SkillPoint:
                    return 1;
                default:
                    return 0;
            }
        }
    }

    public int Level
    {
        get => Data.Load($"{uniqueId}_level", 1);
        set => Data.Save($"{uniqueId}_level", value);
    }
}
