using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEngine;
using UnityEngine.UI;

public class BuyableResource : GameComponent
{
    [SerializeField] private Image resourceIcon;

    public void Initialize(Sprite resourceSprite)
    {
        resourceIcon.sprite = resourceSprite;
        resourceIcon.SetNativeSize();
    }
}
