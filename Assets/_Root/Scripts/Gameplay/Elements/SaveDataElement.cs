using System;
using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;

public class SaveDataElement : GameComponent
{
    [SerializeField, UniqueID] protected string uniqueId;
    
#if UNITY_EDITOR
    [ContextMenu("Reset Unique Id")]
    public void ResetUniqueID()
    {
        Guid guid = Guid.NewGuid();
        uniqueId = guid.ToString();
    }
#endif
}
