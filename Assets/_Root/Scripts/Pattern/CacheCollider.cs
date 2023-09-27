using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CachedCollider
{
    public static Dictionary<Collider, Field> fieldColliderDictionary = new Dictionary<Collider, Field>();

    public static Field GetField(Collider collider)
    {
        if(fieldColliderDictionary.TryGetValue(collider, out Field characterCombat)) return characterCombat;

        fieldColliderDictionary.Add(collider, collider.GetComponent<Field>());
        return fieldColliderDictionary[collider];
    }
}