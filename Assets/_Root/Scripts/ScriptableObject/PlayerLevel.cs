using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Player/PlayerLevel")]
public class PlayerLevel : ScriptableObject
{
    [SerializeField] private int maxLevel = 100;
    [SerializeField] private float expFactor0 = 140;
    [SerializeField] private float expFactor1 = 1.5f;

    public bool IsMaxLevel => Level >= maxLevel;
    public float RequiredExp => IsMaxLevel ? 0 : expFactor0 * Mathf.Pow(expFactor1, Level + 1);
    private bool CanLevelUp => !IsMaxLevel && Exp >= RequiredExp;
    public float ExpUp { get; private set; }

    public Action<float> OnExpChangedEvent;
    public Action<int> OnLevelChangedEvent;
    public Action<int> OnSkillPointChangedEvent;

    public float Exp
    {
        get => Data.Load("player_exp", 0.0f);
        private set
        {
            // Debug.LogError("player_exp" + value);
            Data.Save("player_exp", value);
            OnExpChangedEvent?.Invoke(value);
        }
    }

    public int Level
    {
        get => Data.Load("player_level", 1);
        private set
        {
            Data.Save("player_level", value);
            // Debug.LogError("player_level" + value);
            OnLevelChangedEvent?.Invoke(value);
        }
    }

    public int SkillPoint
    {
        get => Data.Load("player_skill_point", 0);
        private set
        {
            Data.Save("player_skill_point", value);
            OnSkillPointChangedEvent?.Invoke(value);
        }
    }

    public void AddExp(float value)
    {
        // Debug.LogError("add" + value);
        ExpUp = value;
        Exp += value;
        // Debug.LogError("shiba" + Exp);
        if (CanLevelUp)
        {
            // Debug.LogError(Exp + " - " + RequiredExp);
            LevelUp();
        }
    }

    private void LevelUp()
    {
        while (CanLevelUp)
        {
            // Debug.LogError("?" + RequiredExp);
            Exp -= RequiredExp;
            SkillPoint++;
            Level++;
        }
    }

    public void SpendSkillPoint()
    {
        if (SkillPoint <= 0) return;
        SkillPoint--;
    }

#if  UNITY_EDITOR
    public void Add100Exp()
    {
        AddExp(100);
    }
#endif
}

#if  UNITY_EDITOR
[CustomEditor(typeof(PlayerLevel))]
public class PlayerLevelEditor : Editor
{
    private PlayerLevel _playerLevel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _playerLevel = (PlayerLevel)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Add 100 Exp"))
        {
            _playerLevel.Add100Exp();
        }
    }
}
#endif

