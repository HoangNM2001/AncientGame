using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Pancake;
using Pancake.Scriptable;
using Pancake.Threading.Tasks.Triggers;
using Pancake.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Math = System.Math;

public class HMenuController : GameComponent
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private ResourceQuantity coinQuantity;
    [SerializeField] private Transform resourceQuantityParent;
    [SerializeField] private ResourceQuantity resourceQuantityPrefab;

    [Header("POPUP")][SerializeField] private UIButton settingBtn;
    [SerializeField, PopupPickup] private string settingsPopup;

    [Header("EVENT")][SerializeField] private PopupShowEvent popupShowEvent;
    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;
    [SerializeField] private ScriptableEventCoinFlyEventData coinFlyEvent;
    [SerializeField] private ScriptableEventStorageAddData addStorageEvent;

    [SerializeField] private ResourceConfig coinResourceConfig;
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();

    private Dictionary<EnumPack.ResourceType, ResourceQuantity> resourceQuantityDict =
        new Dictionary<EnumPack.ResourceType, ResourceQuantity>();

    private Dictionary<EnumPack.ResourceType, GameObjectPool> resourceFlyUIDict =
        new Dictionary<EnumPack.ResourceType, GameObjectPool>();

    private Camera mainCamera;
    private Camera uiCamera;

    private const int CoinEffectNums = 15;

    private void Awake()
    {
        foreach (var resourceConfig in resourceConfigList)
        {
            var tempQuantity = Instantiate(resourceQuantityPrefab, resourceQuantityParent);
            tempQuantity.Initialize(resourceConfig.resourceIcon, resourceConfig.resourceType,
                resourceConfig.resourceQuantity);
            resourceQuantityDict.Add(tempQuantity.ResourceType, tempQuantity);

            resourceFlyUIDict.Add(resourceConfig.resourceType, resourceConfig.flyUIPool);
            tempQuantity.gameObject.SetActive(tempQuantity.QuantityVariable.Value != 0);
        }

        coinQuantity.Initialize(coinResourceConfig.resourceIcon, coinResourceConfig.resourceType,
            coinResourceConfig.resourceQuantity);

        mainCamera = Camera.main;
        uiCamera = GetComponent<Canvas>().worldCamera;
    }

    protected override void OnEnabled()
    {
        getPopupParentEvent.OnRaised += getPopupParent_OnRaised;
        toggleMenuUIEvent.OnRaised += toggleMenuUIEvent_OnRaised;
        flyUIEvent.OnRaised += flyUIEvent_OnRaised;
        coinFlyEvent.OnRaised += coinFlyEvent_OnRaised;
        addStorageEvent.OnRaised += addStorageEvent_OnRaised;
    }

    private void addStorageEvent_OnRaised(StorageAddData storageAddData)
    {

    }

    private void coinFlyEvent_OnRaised(CoinFlyEventData coinFlyEventData)
    {
        var position = mainCamera.WorldToScreenPoint(coinFlyEventData.worldPos);
        position = uiCamera.ScreenToWorldPoint(position);

        for (int i = 0; i < CoinEffectNums; i++)
        {
            var coinFlyUI = coinResourceConfig.flyUIPool.Request();
            coinFlyUI.transform.SetParent(menuUI.transform);
            var randomDistance = UnityEngine.Random.insideUnitCircle * 150.0f;
            var randomPos = new Vector3(position.x + randomDistance.x, position.y + randomDistance.y, position.z);
            coinFlyUI.transform.position = new Vector3(randomPos.x, randomPos.y, position.z);
            coinFlyUI.GetComponent<ResourceFlyUI>().DoMove(coinQuantity.IconPosition, () =>
            {
                coinResourceConfig.flyUIPool.Return(coinFlyUI);
                coinQuantity.ScaleEffect();
            });
        }

        coinQuantity.UpdateCoinValue(coinFlyEventData.changeValue);

        foreach (var resourceQuantity in resourceQuantityDict.Values.Where(resourceQuantity =>
                     resourceQuantity.QuantityVariable.Value == 0))
        {
            resourceQuantity.ChangeValueTxtEffect(0, () => resourceQuantity.gameObject.SetActive(false));
        }
    }

    private void flyUIEvent_OnRaised(FlyEventData flyEventData)
    {
        var tempQuantity = resourceQuantityDict[flyEventData.resourceType];
        if (!tempQuantity.gameObject.activeInHierarchy) tempQuantity.gameObject.SetActive(true);

        var position = mainCamera.WorldToScreenPoint(flyEventData.worldPos);
        position = uiCamera.ScreenToWorldPoint(position);

        var resourceFlyUI = resourceFlyUIDict[flyEventData.resourceType].Request();
        resourceFlyUI.transform.SetParent(menuUI.transform);
        resourceFlyUI.transform.position = position;
        resourceFlyUI.GetComponent<ResourceFlyUI>().DoMove(tempQuantity.IconPosition, () =>
        {
            resourceFlyUIDict[flyEventData.resourceType].Return(resourceFlyUI);
            tempQuantity.UpdateValue();
        });
    }

    private void toggleMenuUIEvent_OnRaised(bool activeStatus)
    {
        menuUI.gameObject.SetActive(activeStatus);
    }

    private GameObject getPopupParent_OnRaised()
    {
        return gameObject;
    }

    protected override void OnDisabled()
    {
        getPopupParentEvent.OnRaised -= getPopupParent_OnRaised;
        toggleMenuUIEvent.OnRaised -= toggleMenuUIEvent_OnRaised;
        flyUIEvent.OnRaised -= flyUIEvent_OnRaised;
        coinFlyEvent.OnRaised -= coinFlyEvent_OnRaised;
        addStorageEvent.OnRaised -= addStorageEvent_OnRaised;
    }

    private void Start()
    {
        settingBtn.onClick.AddListener(ShowSettingsPopup);
    }

    private void ShowSettingsPopup()
    {
        popupShowEvent.Raise(settingsPopup, transform);
    }

#if UNITY_EDITOR
    [ContextMenu("Get Resources")]
    public void GetResources()
    {
        const string resourcesFolderPath = "Assets/_Root/ScriptableData/Resources";

        var resourcePaths = AssetDatabase.FindAssets("t:ResourceConfig", new string[] { resourcesFolderPath });

        var resourceConfigs = new ResourceConfig[resourcePaths.Length];

        for (var i = 0; i < resourcePaths.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(resourcePaths[i]);
            resourceConfigs[i] = AssetDatabase.LoadAssetAtPath<ResourceConfig>(assetPath);
        }

        resourceConfigList = resourceConfigs.ToList();
        resourceConfigList.Remove(resourceConfigList.FirstOrDefault(x => x.resourceType == EnumPack.ResourceType.Gold));
        EditorUtility.SetDirty(this);
    }
#endif
}

[Serializable]
public class FlyEventData
{
    public EnumPack.ResourceType resourceType;
    public Vector3 worldPos;
}

[Serializable]
public class CoinFlyEventData
{
    public int changeValue;
    public Vector3 worldPos;
}

[Serializable]
public class StorageAddData
{
    public Vector3 startPos;
    public Vector3 endPos;
    public int number;
    public Action onDone;
}