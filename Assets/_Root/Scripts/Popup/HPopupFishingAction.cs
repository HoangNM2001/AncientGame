using System;
using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;

public class HPopupFishingAction : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getFishingFieldEvent;
    [SerializeField] private IntVariable currentMiniGameType;
    [SerializeField] private ScriptableEventBool toggleMiniGame;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventBool toggleMainCamera;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private GameObject fishingUI;
    [SerializeField] private GameObject fishingButtonUI;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private FishingData fishingData;

    protected override void OnBeforeShow()
    {
        fishingData.Reset();
        FishingStateEnable(false);
    }

    public void StartFishingAction()
    {
        currentMiniGameType.Value = (int)EnumPack.MiniGameType.Fishing;
        toggleMiniGame.Raise(true);

        changeInputEvent.Raise((int)EnumPack.ControlType.Vertical);
        toggleMainCamera.Raise(false);
        toggleMenuUIEvent.Raise(false);

        fishingData.OnFishCountCaught += fishingData_OnFishCountCaught;
        fishingData_OnFishCountCaught(fishingData);

        FishingStateEnable(true);
    }

    public void StopFishingAction()
    {
        toggleMiniGame.Raise(false);

        changeInputEvent.Raise((int)EnumPack.ControlType.Move);
        toggleMainCamera.Raise(true);
        toggleMenuUIEvent.Raise(true);

        fishingData.OnFishCountCaught -= fishingData_OnFishCountCaught;

        FishingStateEnable(false);

        if (getFishingFieldEvent.Raise().TryGetComponent<FishingField>(out var fishingField))
        {
            fishingField.HarvestFish();
        }
        
        ClosePopup();
    }

    private void fishingData_OnFishCountCaught(FishingData data)
    {
        countText.text = $"{fishingData.FishCount}/{fishingData.MaxCount}";
    }

    private void FishingStateEnable(bool isFishing)
    {
        fishingUI.SetActive(isFishing);
        fishingButtonUI.SetActive(!isFishing);
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
