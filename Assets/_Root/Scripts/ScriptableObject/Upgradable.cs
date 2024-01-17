using System;
using Pancake;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Upgradable")]
public class Upgradable : ScriptableObject
{
    [ID] public string uniqueId;
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
    public Sprite background;
    public Sprite icon;

    public Action OnUpgraded;

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
        private set => Data.Save($"{uniqueId}_level", value);
    }

    public void Upgrade()
    {
        if (!IsUpgradable) return;
        switch (costType)
        {
            case EnumPack.CostType.Coin:
                coinQuantity.Value -= Cost;
                break;
            case EnumPack.CostType.SkillPoint:
                playerLevel.SpendSkillPoint();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        Level++;
        OnUpgraded?.Invoke();
    }

#if UNITY_EDITOR
    public void TestUpgrade()
    {
        Level++;
        Debug.LogError(Level);
        OnUpgraded?.Invoke();
    }
#endif
    
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        uniqueId = IDAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Upgradable))]
public class UpgradableEditor : Editor
{
    private Upgradable _upgradable;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _upgradable = (Upgradable)target;

        EditorGUILayout.Space();

        if (GUILayout.Button($"Upgrade {_upgradable.Level + 1}"))
            // if (GUILayout.Button("Upgrade"))
        {
            _upgradable.TestUpgrade();
        }
    }
}
#endif