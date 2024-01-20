using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupCave : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCurrentCaveEvent;

    private ResourcesCave currentCave;

    protected override void OnBeforeShow()
    {
        currentCave = getCurrentCaveEvent.Raise().GetComponent<ResourcesCave>();
    }

    public void CollectResource()
    {
        currentCave.CollectStorage();
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}