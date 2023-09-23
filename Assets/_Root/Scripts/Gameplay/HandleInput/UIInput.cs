using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using Pancake.Component;
using Pancake.Scriptable;

public class UIInput : GameComponent
{
    [SerializeField] private Joystick joystick;
    [SerializeField] private Vector2Variable moveDirection;

    public Vector3 MoveDirection => moveDirection.Value = joystick.Direction;
}
