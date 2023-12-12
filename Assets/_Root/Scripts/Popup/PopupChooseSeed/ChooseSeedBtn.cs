using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using UnityEngine;
using UnityEngine.UI;

public class ChooseSeedBtn : ButtonAction
{
    [SerializeField] private Image fruitIcon;
    [SerializeField] private ScriptableEventGetGameObject getCurrentExtendFieldEvent;
    
    private EnumPack.ResourceType resourceType;

    public void Initialize(ResourceConfig resourceConfig)
    {
        fruitIcon.sprite = resourceConfig.resourceIcon;
        fruitIcon.SetNativeSize();

        resourceType = resourceConfig.resourceType;
    }

    protected override void StartAction()
    {
        base.StartAction();

        var currentExtendField = getCurrentExtendFieldEvent.Raise().GetComponent<ExtendField>();
        if (currentExtendField == null) return;

        currentExtendField.InitializeOnNewSeed(resourceType);
    }
}
