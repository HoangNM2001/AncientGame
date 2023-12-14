using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPopupHuntingAction : UIPopup
{
    [SerializeField] private ScriptableEventNoParam forceStopMiniGame;
    [SerializeField] private ScriptableEventGetGameObject getHuntingFieldEvent;
    [SerializeField] private IntVariable currentMiniGameType;
    [SerializeField] private ScriptableEventBool toggleMiniGame;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventBool toggleMainCamera;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private PredatorVariable predatorVariable;
    [SerializeField] private GameObject huntingUI;
    [SerializeField] private GameObject huntingButton;
    [SerializeField] private Image predatorImage;
    [SerializeField] private Image healthBar;
    [SerializeField] private CameraVariable mainCameraVariable;
    [SerializeField] private CameraVariable uiCameraVariable;
    [SerializeField] private GameObject hitNotification;

    private MapPredator mapPredator;

    protected override void OnEnabled()
    {
        forceStopMiniGame.OnRaised += StopHuntingAction;
    }

    protected override void OnDisabled()
    {
        forceStopMiniGame.OnRaised -= StopHuntingAction;
    }

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

        if (getHuntingFieldEvent.Raise().TryGetComponent<HuntingField>(out var huntingField))
        {
            huntingField.HarvestOnWin();
        }

        OnDeActiveMiniGame();
        ClosePopup();
    }

    private void HuntingStateEnable(bool isHunting)
    {
        huntingUI.SetActive(isHunting);
        huntingButton.SetActive(!isHunting);
    }

    private void OnActiveMiniGame()
    {
        healthBar.fillAmount = 1.0f;
        predatorImage.sprite = predatorVariable.Value.PredatorIcon;

        predatorVariable.Value.OnHpChangeEvent += OnHpChangeEvent;
        predatorVariable.Value.OnShowNextHitPointEvent += OnShowNextHitPointEvent;
    }

    private void OnDeActiveMiniGame()
    {
        predatorVariable.Value.OnHpChangeEvent -= OnHpChangeEvent;
        predatorVariable.Value.OnShowNextHitPointEvent -= OnShowNextHitPointEvent;
    }

    private void OnHpChangeEvent()
    {
        healthBar.fillAmount = (float)predatorVariable.Value.CurrentHp / predatorVariable.Value.MaxHp;
    }

    private void OnShowNextHitPointEvent(Vector3 worldPos)
    {
        var position = mainCameraVariable.Value.WorldToScreenPoint(worldPos);
        position = uiCameraVariable.Value.ScreenToWorldPoint(position);

        Debug.LogError(position);
        hitNotification.transform.position = position;
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