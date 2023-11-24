using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumPack
{
    public enum MiniGameType
    {
        Fishing,
        Hunting
    }

    public enum CharacterActionType
    {
        SeedFarm,
        WaterFarm,
        HarvestFarm,
        HarvestFruit,
        Fishing,
        Hunting,
    }

    public enum ControlType
    {
        Move,
        Horizontal, 
        Vertical
    }
    
    public enum FieldState
    {
        Seedale,
        Waterable,
        Harvestable,
    }

    public enum ResourceType
    {
        Gold,
        Diamond,
        Corn,
        Apple,
        Watermelon,
        Any,
        Coconut,
        Egg,
        Rice,
        Banana,
        RayFish,
        FatFish,
        LanternFish,
        Carrot,
        Chilli,
        Pumpkin,
        Orange,
        Peach,
        Mango,
        Aubergine,
        Strawberry,
        Cotton,
        Cabbage,
        Radish,
        Sugarcane,
        Potato,
        Meat,
        None
    }
}
