using System.Collections;
using System.Collections.Generic;
using Pancake;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyableResource : GameComponent
{
    [SerializeField] private Image resourceIcon;
    [SerializeField] private TextMeshProUGUI valueText;

    public void Initialize(Sprite resourceSprite, int value)
    {
        resourceIcon.sprite = resourceSprite;
        resourceIcon.SetNativeSize();

        valueText.text = value.ToString();
    }
}
