using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Character/Statistics")]
public class CharacterStat : ScriptableObject
{
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 360.0f;
    public float workingSpeed = 1.0f;
}