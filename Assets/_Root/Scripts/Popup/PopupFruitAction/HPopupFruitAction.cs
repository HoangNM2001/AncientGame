using System.Collections;
using System.Collections.Generic;
using Pancake.SceneFlow;
using Pancake.Scriptable;
using Pancake.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HPopupFruitAction : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventGetGameObject getCurrentTreeEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventNoParam harvestFruitEvent;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private Vector2Variable shakeInput;
    [SerializeField] private GameObject shakeUI;
    [SerializeField] private GameObject fruitButtonUI;
    [SerializeField] private Image fruitFillBar;
    [SerializeField] private TextMeshProUGUI fillText;
    [SerializeField] private List<FruitActionBtn> fruitBtnList;

    private CharacterController characterController;
    private FruitTree currentTree;
    private bool isShaking;
    private bool shakeable;

    protected override void OnEnabled()
    {
        harvestFruitEvent.OnRaised += harvestFruitEvent_OnRaised;
    }

    private void harvestFruitEvent_OnRaised()
    {
        currentTree.Shake();
        UpdateShakeProcessBar();
    }

    protected override void OnDisabled()
    {
        harvestFruitEvent.OnRaised -= harvestFruitEvent_OnRaised;
    }

    private void Start()
    {
        characterController = getCharacterEvent.Raise().GetComponent<CharacterController>();
    }

    protected override void OnBeforeShow()
    {
        IsShakingState(false);

        currentTree = getCurrentTreeEvent.Raise().GetComponent<FruitTree>();
        if (currentTree == null) return;
        
        foreach (var fruitBtn in fruitBtnList)
        {
            fruitBtn.gameObject.SetActive(currentTree.FruitResource.resourceType == fruitBtn.FruitType);
        }
    }

    protected override void Tick()
    {
        if (!isShaking) return;

        if (!SimpleMath.InRange(characterController.transform.position, currentTree.StandPosition.position, 0.2f))
        {
            characterController.MoveToPosition(currentTree.StandPosition.position,
                characterController.CharacterStat.workingMoveSpeed, Time.deltaTime);
        }
        else
        {
            characterController.RotateToTarget(currentTree.LookAtPosition.position, Time.deltaTime);
            Shaking();
        }
    }

    private void Shaking()
    {
        if (shakeInput.Value.x >= -0.1f && shakeInput.Value.x <= 0.1f) shakeable = true;
        if (shakeable && (shakeInput.Value.x >= 0.75f || shakeInput.Value.x <= -0.75))
        {
            shakeable = false;
            characterController.CharacterAnimController.Play(Constant.HARVEST_FRUIT, 1);
        }
    }

    public void StartFruitAction()
    {
        IsShakingState(true);
        SetShakeProcessbar();
        
        currentTree.GrownFruitHandle.Pause();
        changeInputEvent.Raise((int)EnumPack.ControlType.Horizontal);
        toggleMenuUIEvent.Raise(false);
    }

    private void SetShakeProcessbar()
    {
        fruitFillBar.fillAmount = (float)currentTree.CurrentFruitQuantity / currentTree.MaxFruitQuantity;
        fillText.text = currentTree.CurrentFruitQuantity + "/" + currentTree.MaxFruitQuantity;
    }

    private void UpdateShakeProcessBar()
    {
        var targetFillAmount = (float)currentTree.CurrentFruitQuantity / currentTree.MaxFruitQuantity;
        
        fruitFillBar.DOFillAmount(targetFillAmount, 0.8f) 
            .OnUpdate(() => {
                fillText.text = Mathf.RoundToInt(fruitFillBar.fillAmount * currentTree.MaxFruitQuantity) + "/" + currentTree.MaxFruitQuantity;
            });
    }

    private void IsShakingState(bool shakingValue)
    {
        isShaking = shakingValue;
        shakeUI.SetActive(isShaking);
        fruitButtonUI.SetActive(!isShaking);
    }

    public void ClosePopup()
    {
        characterController.CharacterAnimController.Play(Constant.EMPTY, 1);
        changeInputEvent.Raise((int)EnumPack.ControlType.Move);
        toggleMenuUIEvent.Raise(true);
        currentTree.GrownFruitHandle.Resume();
        currentTree.ReturnDroppedModel();
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}