using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupFruitAction : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCharacterEvent;
    [SerializeField] private ScriptableEventGetGameObject getCurrentTreeEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private GameObject shakeUI;
    [SerializeField] private GameObject fruitButtonUI;
    [SerializeField] private List<FruitActionBtn> fruitBtnList;

    private FruitTree currentTree;
    private bool isShaking;
    private bool startShake;
    private CharacterController characterController;

    private void Start()
    {
        characterController = getCharacterEvent.Raise().GetComponent<CharacterController>();
    }

    protected override void OnBeforeShow()
    {
        IsShakingState(false);

        startShake = false;

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
        if (startShake) return;
        if (!SimpleMath.InRange(characterController.transform.position, currentTree.StandPosition, 0.1f))
        {
            characterController.MoveToPosition(currentTree.StandPosition, characterController.CharacterStat.workingMoveSpeed, Time.deltaTime);
            Debug.LogError("Move");
        }
        else
        {
            characterController.transform.rotation = currentTree.transform.rotation;
            startShake = true;
            Debug.LogError("Rotate");
        }
        // Debug.LogError("Run?");
    }

    public void StartFruitAction()
    {
        IsShakingState(true);

        changeInputEvent.Raise((int)EnumPack.ControlType.Horizontal);

        // var test = SimpleMath.InRange(characterController.transform.position, currentTree.StandPosition, 0.2f);
        // var test = new Vector3(characterController.transform.position.x - currentTree.StandPosition.x, 0.0f, characterController.transform.position.z - currentTree.StandPosition.z);
        // Debug.LogError(test.sqrMagnitude);
    }

    private void IsShakingState(bool shakingValue)
    {
        isShaking = shakingValue;
        shakeUI.SetActive(isShaking);
        fruitButtonUI.SetActive(!isShaking);
    }

    public void ClosePopup()
    {
        changeInputEvent.Raise((int)EnumPack.ControlType.Move);
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
    }
}
