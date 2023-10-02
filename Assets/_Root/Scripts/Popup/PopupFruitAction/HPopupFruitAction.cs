using System.Collections;
using System.Collections.Generic;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEngine;

public class HPopupFruitAction : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCurrentTreeEvent;
    [SerializeField] private ScriptableEventInt changeInputEvent;
    [SerializeField] private ScriptableEventVector3 moveToTreeEvent;
    [SerializeField] private GameObject shakeUI;
    [SerializeField] private GameObject fruitButtonUI;
    [SerializeField] private List<FruitActionBtn> fruitBtnList;

    private FruitTree currentTree;

    protected override void OnBeforeShow()
    {
        ButtonState();
        
        currentTree = getCurrentTreeEvent.Raise().GetComponent<FruitTree>();
        if (currentTree == null) return;

        foreach (var fruitBtn in fruitBtnList)
        {
            fruitBtn.gameObject.SetActive(currentTree.FruitResource.resourceType == fruitBtn.FruitType);
        }
    }

    public void StartFruitAction()
    {
        ShakeState();
        
        changeInputEvent.Raise((int)EnumPack.ControlType.Horizontal);
        // moveToTreeEvent.Raise(currentTree.StandPosition);
    }

    private void ShakeState()
    {
        shakeUI.SetActive(true);
        fruitButtonUI.SetActive(false);
    }
    private void ButtonState()
    {
        shakeUI.SetActive(false);
        fruitButtonUI.SetActive(true);
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
