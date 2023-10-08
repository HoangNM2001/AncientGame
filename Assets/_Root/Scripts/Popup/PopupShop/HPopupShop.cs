using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake.Scriptable;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

public class HPopupShop : UIPopup
{
    [SerializeField] private ScriptableEventGetGameObject getCurrentShopEvent;
    [SerializeField] private ScriptableEventCoinFlyEventData coinFlyEvent;
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();
    [SerializeField] private List<ResourceConfig> buyAbleResourceList = new List<ResourceConfig>();

    private ShopResources currentShop;

    protected override void OnBeforeShow()
    {
        currentShop = getCurrentShopEvent.Raise().GetComponent<ShopResources>();
        if (currentShop == null) return;

        buyAbleResourceList = currentShop.BuyAbleResourceList;
        // currentShop.IsShowSellableUI(false);
    }

    public void SellAll()
    {
        int total = 0;
        foreach (var resource in buyAbleResourceList)
        {
            total += resource.resourceQuantity.Value * resource.Price;
            // resource.resourceQuantity.Value = 0;
        }

        if (total > 0)
        {
            coinFlyEvent.Raise(new CoinFlyEventData
            {
                changeValue = total,
                worldPos = currentShop.ShopPos
            });
        }

        ClosePopup();
    }

    // protected override void OnBeforeClose()
    // {
    //     currentShop.IsShowSellableUI(true);
    // }

    public void ClosePopup()
    {
        closePopupEvent.Raise();
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
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
