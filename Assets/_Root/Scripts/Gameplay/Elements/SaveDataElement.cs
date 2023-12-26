using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class SaveDataElement : GameComponent
{
    [SerializeField, UniqueID] protected string uniqueId;

    public bool IsUnlocked
    {
        get => Data.Load(uniqueId + "isUnlocked", false);
        set => Data.Save(uniqueId + "isUnlocked", value);
    }
    
#if UNITY_EDITOR
    [ContextMenu("Reset Unique Id")]
    public void ResetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        uniqueId = guid.ToString();
    }
#endif
}
