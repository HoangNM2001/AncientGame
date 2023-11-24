using System.Collections;
using System.Collections.Generic;
using Pancake.SceneFlow;
using UnityEngine;

public interface ICharacterAction
{
    FarmTool FarmTool { get; }
    EnumPack.CharacterActionType CharacterActionType { get; }
    bool Activated { get; }

    void Activate();
    void Deactivate();
}
