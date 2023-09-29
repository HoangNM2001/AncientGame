using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSeedBtn : ButtonAction
{
    [SerializeField] private Image fruitIcon;
    
    private EnumPack.ResourceType resourceType;

    public void Initialize(ResourceConfig resourceConfig)
    {
        fruitIcon.sprite = resourceConfig.resourceIcon;
        fruitIcon.SetNativeSize();
    }
}
