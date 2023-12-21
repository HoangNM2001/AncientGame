using System.Collections;
using System.Collections.Generic;
using Pancake;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using UnityEngine;

public class Egg : GameComponent
{
    private Henhouse _henhouse;

    public void Setup(Henhouse henhouse)
    {
        _henhouse = henhouse;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constant.MAIN_TAG))
        {
            _henhouse.HarvestEgg(this);
        }
    }
}
