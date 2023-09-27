using System.Collections;
using System.Collections.Generic;
using Pancake.SceneFlow;
using UnityEngine;

public interface ICharacterAction
{
    EnumPack.CharacterActionType CharacterActionType { get; }
    bool Activated { get; }
    void Activate();
    void Deactivate();
}
