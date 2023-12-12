using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;

public class HPopupSaveSlave : UIPopup
{
    [SerializeField] private ResourceConfig coinResource;
    [SerializeField] private ScriptableEventGetGameObject getCurrentSaveSlaveEvent;
    [SerializeField] private TextMeshProUGUI priceText;

    private SlaveElement currentSlaveElement;

    protected override void OnBeforeShow()
    {
        currentSlaveElement = getCurrentSaveSlaveEvent.Raise().GetComponent<SlaveElement>();
        priceText.SetText(currentSlaveElement.Price.ToString());
    }

    public void UnlockSlave()
    {
        currentSlaveElement.UnlockSlave();
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
