using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniGame
{
    public EnumPack.MiniGameType MiniGameType { get; }
    public void Activate();
    public void Deactivate();
}
