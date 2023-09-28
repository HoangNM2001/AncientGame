using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

public class ExtendField : GameComponent
{
    [SerializeField, ID] private string fieldID;
    [SerializeField] private ScriptableListInt fieldStateList;
    [SerializeField] private List<Field> fieldList = new List<Field>();
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();

    private EnumPack.ResourceType resourceType;

    public void CalculateExtendFieldState()
    {
        fieldStateList.Reset();

        foreach (var field in fieldList)
        {
            CheckFieldState(field, EnumPack.FieldState.Seedale);
            CheckFieldState(field, EnumPack.FieldState.Waterable);
            CheckFieldState(field, EnumPack.FieldState.Harvestable);
        }
    }

    private void CheckFieldState(Field field, EnumPack.FieldState fieldState)
    {
        if (field.FieldState == fieldState && !fieldStateList.Contains((int)fieldState))
        {
            fieldStateList.Add((int)fieldState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CalculateExtendFieldState();
        CharacterHandleTrigger characterHandleTrigger = other.GetComponent<CharacterHandleTrigger>();
        if (characterHandleTrigger) characterHandleTrigger.TriggerActionFarm();
    }

    private void OnTriggerExit(Collider other)
    {
        CharacterHandleTrigger characterHandleTrigger = other.GetComponent<CharacterHandleTrigger>();
        if (characterHandleTrigger) characterHandleTrigger.ExitTriggerActionFarm();
    }

#if UNITY_EDITOR
    [ContextMenu("Reset Id")]
    public void ResetId()
    {
        fieldID = IDAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
    }

    [ContextMenu("Get Children")]
    public void GetChildren()
    {
        fieldList = GetComponentsInChildren<Field>().ToList();
        EditorUtility.SetDirty(this);
    }
#endif
}
