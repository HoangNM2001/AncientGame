using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.Scriptable;
using UnityEngine;

public class FruitActionBtn : GameComponent
{
    [SerializeField] private EnumPack.ResourceType fruitType;

    public EnumPack.ResourceType FruitType => fruitType;
}
