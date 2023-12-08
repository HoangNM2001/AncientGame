using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPopupHuntingAction : UIPopup
{
    [SerializeField] private IntVariable currentMiniGameType;
    [SerializeField] private ScriptableEventBool toggleMiniGame;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventBool toggleMainCamera;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private PredatorVariable predatorVariable;
    [SerializeField] private GameObject huntingUI;
    [SerializeField] private GameObject hungtingButton;
    [SerializeField] private Image predatorImage;
    [SerializeField] private Image healthBar;

    private MapPredator mapPredator;

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
        OnActiveMiniGame();
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

    private void OnActiveMiniGame()
    {
        healthBar.fillAmount = 1.0f;
        predatorImage.sprite = predatorVariable.Value.PredatorIcon;

        predatorVariable.Value.OnHpChangeEvent += OnHpChangeEvent;
    }

    private void OnDeactiveMiniGame()
    {
        predatorVariable.Value.OnHpChangeEvent -= OnHpChangeEvent;
    }

    private void OnHpChangeEvent()
    {
        healthBar.fillAmount = (float)predatorVariable.Value.CurrentHp /  (float)predatorVariable.Value.MaxHp;
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
