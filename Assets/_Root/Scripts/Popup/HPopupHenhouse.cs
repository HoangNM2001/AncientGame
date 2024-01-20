using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupHenhouse : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCurrentCaveEvent;
    [SerializeField] private IntVariable coinQuantity;

    private Henhouse currentHenHouse;

    protected override void OnBeforeShow()
    {
        currentHenHouse = getCurrentCaveEvent.Raise().GetComponent<Henhouse>();
    }

    public void ByMoreChicken()
    {
        currentHenHouse.SpawnChicken();
        coinQuantity.Value -= 500;
    }

    public void HarvestAll()
    {
        currentHenHouse.HarvestAllEggs();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
