using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Character/Statistics")]
public class CharacterStat : ScriptableObject
{
    [SerializeField] protected float rotateSpeed = 20.0f;
    [SerializeField] protected float baseMoveSpeed = 5.0f;
    [SerializeField] protected float baseWorkMoveSpeed = 3.0f;
    [SerializeField] protected float baseWorkSpeed = 0.8f;

    public float RotateSpeed => rotateSpeed;
    public virtual float MoveSpeed => baseMoveSpeed;
    public virtual float WorkMoveSpeed => baseWorkMoveSpeed;
    public virtual float WorkSpeed => baseWorkSpeed;
}