using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FishingData")]
public class FishingData : ScriptableObject
{
    [SerializeField, UniqueID] private string uniqueId;

    public int FishCount;
    public int MaxCount;
    public List<Fish> FishList;

    public Action<FishingData> OnFishCountCaught;
    [HideInInspector] public List<ResourceConfig> caughtList = new List<ResourceConfig>();

    public void Reset()
    {
        FishCount = MaxCount;
    }

    public void Spend()
    {
        if (FishCount > 0)
        {
            FishCount--;
            OnFishCountCaught?.Invoke(this);
        }
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
