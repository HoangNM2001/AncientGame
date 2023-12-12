using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;

public class HPopupSaveSlave : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCurrentSaveSlaveEvent;
    [SerializeField] private TextMeshProUGUI priceText;

    private SlaveElement _currentSlaveElement;

    protected override void OnBeforeShow()
    {
        _currentSlaveElement = getCurrentSaveSlaveEvent.Raise().GetComponent<SlaveElement>();
        priceText.SetText(_currentSlaveElement.Price.ToString());
    }

    public void UnlockSlave()
    {
        _currentSlaveElement.UnlockSlave();
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
