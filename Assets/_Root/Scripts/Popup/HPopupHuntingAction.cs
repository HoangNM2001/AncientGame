using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupHuntingAction : UIPopup
{
    [SerializeField] private IntVariable currentMiniGameType;
    [SerializeField] private ScriptableEventBool toggleMiniGame;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventBool toggleMainCamera;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private GameObject huntingUI;
    [SerializeField] private GameObject hungtingButton;

    protected override void OnBeforeShow()
    {
        HuntingStateEnable(false);
    }

    public void StartHuntingAction()
    {
        currentMiniGameType.Value = (int)EnumPack.MiniGameType.Hunting;
        toggleMiniGame.Raise(true);

        changeInputEvent.Raise((int)EnumPack.ControlType.MiniGame);
        toggleMainCamera.Raise(false);
        toggleMenuUIEvent.Raise(false);

        HuntingStateEnable(true);
    }

    public void StopHuntingAction()
    {
        toggleMiniGame.Raise(false);

        changeInputEvent.Raise((int)EnumPack.ControlType.Move);
        toggleMainCamera.Raise(true);
        toggleMenuUIEvent.Raise(true);

        HuntingStateEnable(false);

        ClosePopup();
    }

    private void HuntingStateEnable(bool isHunting)
    {
        huntingUI.SetActive(isHunting);
        hungtingButton.SetActive(!isHunting);
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
