using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupBuilding : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;

    private PlayerController characterController;

    private void Start()
    {
        characterController = getCharacterEvent.Raise().GetComponent<PlayerController>();
    }

    public void StartBuildAction()
    {
        characterController.CheckToBuild();
        ClosePopup();
    }

    public void ClosePopup()
    {
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
