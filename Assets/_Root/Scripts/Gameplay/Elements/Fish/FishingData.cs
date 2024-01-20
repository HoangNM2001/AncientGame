using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/FishingData")]
public class FishingData : ScriptableObject
{
    public int FishCount;
    public int MaxCount;
    public List<Fish> FishList;

    public Action<FishingData> OnFishCountCaught;
    [HideInInspector] public List<ResourceConfig> caughtList = new();

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
}
