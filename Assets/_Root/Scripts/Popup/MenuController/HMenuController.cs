using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

public class HMenuController : GameComponent
{
    [SerializeField] private GameObject menuUI;
    [SerializeField] private Transform resourceQuantityParent;
    [SerializeField] private ResourceQuantity resourceQuantityPrefab;

    [Header("POPUP")] [SerializeField] private UIButton settingBtn;
    [SerializeField, PopupPickup] private string settingsPopup;

    [Header("EVENT")] [SerializeField] private PopupShowEvent popupShowEvent;
    [SerializeField] private ScriptableEventGetGameObject getPopupParentEvent;
    [SerializeField] private ScriptableEventBool toggleMenuUIEvent;
    [SerializeField] private ScriptableEventFlyEventData flyUIEvent;

    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();

    private Dictionary<EnumPack.ResourceType, ResourceQuantity> resourceQuantityDict =
        new Dictionary<EnumPack.ResourceType, ResourceQuantity>();

    private Dictionary<EnumPack.ResourceType, GameObjectPool> resourceFlyUIDict =
        new Dictionary<EnumPack.ResourceType, GameObjectPool>();

    private Camera mainCamera;
    private Camera uiCamera;

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

        mainCamera = Camera.main;
        uiCamera = GetComponent<Canvas>().worldCamera;
    }

    protected override void OnEnabled()
    {
        getPopupParentEvent.OnRaised += getPopupParent_OnRaised;
        toggleMenuUIEvent.OnRaised += toggleMenuUIEvent_OnRaised;
        flyUIEvent.OnRaised += flyUIEvent_OnRaised;
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
        const string resourcesFolderPath = "Assets/_Root/Resources/ScriptableData/Resources";

        var resourcePaths = AssetDatabase.FindAssets("t:ResourceConfig", new string[] { resourcesFolderPath });

        var resourceConfigs = new ResourceConfig[resourcePaths.Length];

        for (var i = 0; i < resourcePaths.Length; i++)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(resourcePaths[i]);
            resourceConfigs[i] = AssetDatabase.LoadAssetAtPath<ResourceConfig>(assetPath);
        }

        resourceConfigList = resourceConfigs.ToList();
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