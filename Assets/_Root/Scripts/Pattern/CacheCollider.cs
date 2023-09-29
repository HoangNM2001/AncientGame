using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CacheCollider
{
    private static Dictionary<Collider, Field> fieldColliderDictionary = new Dictionary<Collider, Field>();
    private static Dictionary<Collider, ExtendField> extendFieldDictionary = new Dictionary<Collider, ExtendField>();
    private static Dictionary<Collider, CharacterHandleTrigger> handleTriggerDictionary =
        new Dictionary<Collider, CharacterHandleTrigger>();

    public static Field GetField(Collider collider)
    {
        if(fieldColliderDictionary.TryGetValue(collider, out Field field)) return field;

        fieldColliderDictionary.Add(collider, collider.GetComponent<Field>());
        return fieldColliderDictionary[collider];
    }
    
    public static ExtendField GetExtendField(Collider collider)
    {
        if(extendFieldDictionary.TryGetValue(collider, out ExtendField extendField)) return extendField;

        extendFieldDictionary.Add(collider, collider.GetComponent<ExtendField>());
        return extendFieldDictionary[collider];
    }

    public static CharacterHandleTrigger GetCharacterHandleTrigger(Collider collider)
    {
        if (handleTriggerDictionary.TryGetValue(collider, out CharacterHandleTrigger characterHandleTrigger)) return characterHandleTrigger;
        
        handleTriggerDictionary.Add(collider, collider.GetComponent<CharacterHandleTrigger>());
        return handleTriggerDictionary[collider];
    }
}