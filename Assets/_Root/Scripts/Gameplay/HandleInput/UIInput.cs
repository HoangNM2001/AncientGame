using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using Pancake.Component;
using Pancake.Scriptable;

public class UIInput : GameComponent
{
    [SerializeField] private EnumPack.ControlType controlType;

    public EnumPack.ControlType ControlType => controlType;
}
