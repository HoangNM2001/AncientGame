using System.Collections;
using System.Collections.Generic;
using System.IO;
using Pancake;
using UnityEngine;

public class MiniGameFishing : GameComponent, IMiniGame
{
    [SerializeField] private EnumPack.MiniGameType miniGameType;
    [SerializeField] private GameObject container;
    [SerializeField] private Transform fishContainer;
    [SerializeField] private FishingController fishingController;
    [SerializeField] private FishingData fishingData;
    [SerializeField] private LeftRightCouple[] fishLineList;

    public EnumPack.MiniGameType MiniGameType => miniGameType;

    private List<Fish> fishPrefabList;
    private Dictionary<LeftRightCouple, Fish> fishDict;

    private void Awake()
    {
        fishPrefabList = new List<Fish>(fishingData.FishList);
        fishDict = new Dictionary<LeftRightCouple, Fish>();

        foreach (var leftRightCouple in fishLineList)
        {
            fishDict.Add(leftRightCouple, null);
        }
    }

    public void Activate()
    {
        container.SetActive(true);
        fishingController.Activate();

        foreach (var leftRightCouple in fishLineList)
        {
            if (!fishDict[leftRightCouple])
            {
                SpawnFish(leftRightCouple, true);
            }
        }
    }

    public void Deactivate()
    {
        container.SetActive(false);
        fishingController.Deactivate();
    }

    public void OnFishCaught(Fish fish)
    {
        fishingData.caughtList.Add(fish.ResourceConfig);

        if (fishDict.ContainsKey(fish.LeftRightCouple)) SpawnFish(fish.LeftRightCouple, false);
    }

    private void SpawnFish(LeftRightCouple leftRightCouple, bool isRestore)
    {
        var fish = Instantiate(fishPrefabList[Random.Range(0, fishPrefabList.Count)], fishContainer);
        fish.Activate(this, leftRightCouple, isRestore);

        fishDict[leftRightCouple] = fish;
    }
}
