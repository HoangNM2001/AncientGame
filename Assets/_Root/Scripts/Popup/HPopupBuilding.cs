using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupBuilding : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;

    private PlayerController _characterController;

    private void Start()
    {
        _characterController = getCharacterEvent.Raise().GetComponent<PlayerController>();
    }

    public void StartBuildAction()
    {
        _characterController.CheckToBuild();
        ClosePopup();
    }

    private void ClosePopup()
    {
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
