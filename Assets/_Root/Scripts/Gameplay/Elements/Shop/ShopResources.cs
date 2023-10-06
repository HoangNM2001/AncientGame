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
        
        ScaleObject(sellableUI.transform, true);
        shopKeeperAnimator.CrossFade(Constant.SHOPKEEPER_WAVE, 0.1f);
    }

    private void ExitTriggerWave(Collider obj)
    {
        if (characterHandleTrigger == null) characterHandleTrigger = CacheCollider.GetCharacterHandleTrigger(obj);
        characterHandleTrigger.ExitTriggerActionShopFar(gameObject);
        
        ScaleObject(sellableUI.transform, false);
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

    private void ScaleObject(Transform targetTrans, bool isAppear)
    {
        if (isAppear)
        {
            targetTrans.gameObject.SetActive(true);
            targetTrans.DOScale(defaultUIScale, 0.25f);
        }
        else
        {
            targetTrans.DOScale(0.0f, 0.25f).OnComplete(() => targetTrans.gameObject.SetActive(false));
        }
    }
}