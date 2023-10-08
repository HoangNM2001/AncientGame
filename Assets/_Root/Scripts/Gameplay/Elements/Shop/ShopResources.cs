using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pancake;
using Pancake.SceneFlow;
using UnityEditor;
using UnityEngine;

public class ShopResources : GameComponent
{
    [SerializeField] private List<ResourceConfig> buyAbleResourceList;
    [SerializeField] private Animator shopKeeperAnimator;
    [SerializeField] private GameObject sellableUI;
    [SerializeField] private Transform buyAbleParent;
    [SerializeField] private BuyableResource buyAbleResourcePrefab;
    [SerializeField] private Trigger triggerWave;
    [SerializeField] private Trigger triggerTalk;

    private CharacterHandleTrigger characterHandleTrigger;
    private Vector3 defaultUIScale;

    public Vector3 ShopPos => shopKeeperAnimator.transform.position;
    public List<ResourceConfig> BuyAbleResourceList => buyAbleResourceList;
    private void Start()
    {
        foreach (var resource in buyAbleResourceList)
        {
            Instantiate(buyAbleResourcePrefab, buyAbleParent).Initialize(resource.resourceIcon);
        }
        
        defaultUIScale = sellableUI.transform.localScale;
        sellableUI.SetActive(false);
        sellableUI.transform.localScale = Vector3.zero;
    }

    protected override void OnEnabled()
    {
        triggerWave.EnterTriggerEvent += OnTriggerWave;
        triggerWave.ExitTriggerEvent += ExitTriggerWave;
        triggerTalk.EnterTriggerEvent += OnTriggerTalk;
        triggerTalk.ExitTriggerEvent += ExitTriggerTalk;
    }

    protected override void OnDisabled()
    {
        triggerWave.EnterTriggerEvent -= OnTriggerWave;
        triggerWave.ExitTriggerEvent -= ExitTriggerWave;
        triggerTalk.EnterTriggerEvent -= OnTriggerTalk;
        triggerTalk.ExitTriggerEvent -= ExitTriggerTalk;
    }

    private void OnTriggerWave(Collider obj)
    {
        if (characterHandleTrigger == null) characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(obj);
        characterHandleTrigger.TriggerActionShopFar(gameObject);
        
        IsShowSellableUI(true);
        shopKeeperAnimator.CrossFade(Constant.SHOPKEEPER_WAVE, 0.1f);
    }

    private void ExitTriggerWave(Collider obj)
    {
        if (characterHandleTrigger == null) characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(obj);
        characterHandleTrigger.ExitTriggerActionShopFar(gameObject);
        
        IsShowSellableUI(false);
        shopKeeperAnimator.CrossFade(Constant.SHOPKEEPER_IDLE, 0.1f);
    }

    private void OnTriggerTalk(Collider obj)
    {
        if (characterHandleTrigger == null) characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(obj);
        characterHandleTrigger.TriggerActionShopNear(gameObject);
        
        shopKeeperAnimator.CrossFade(Constant.SHOPKEEPER_TALKING, 0.1f);
    }

    private void ExitTriggerTalk(Collider obj)
    {
        if (characterHandleTrigger == null) characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(obj);
        characterHandleTrigger.ExitTriggerActionShopNear(gameObject);
        
        shopKeeperAnimator.CrossFade(Constant.SHOPKEEPER_IDLE, 0.1f);
    }

    public void IsShowSellableUI(bool isAppear)
    {
        if (isAppear)
        {
            sellableUI.gameObject.SetActive(true);
            sellableUI.transform.DOScale(defaultUIScale, 0.25f);
        }
        else
        {
            sellableUI.transform.DOScale(0.0f, 0.25f).OnComplete(() => sellableUI.transform.gameObject.SetActive(false));
        }
    }
}