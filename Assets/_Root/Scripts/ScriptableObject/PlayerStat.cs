using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Player/Statistics")]
public class PlayerStat : CharacterStat
{
    [SerializeField] private Upgradable moveUpgradable;
    [SerializeField] private Upgradable workUpgradable;

    public override float MoveSpeed => moveUpgradable.Value * baseMoveSpeed;
    public override float WorkMoveSpeed => moveUpgradable.Value * baseWorkMoveSpeed;
    public override float WorkSpeed => workUpgradable.Value * baseWorkSpeed;
    public int ToolIndex => workUpgradable.Level / 10;
}
