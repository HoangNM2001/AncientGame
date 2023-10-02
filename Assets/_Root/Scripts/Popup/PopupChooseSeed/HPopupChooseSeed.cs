using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake;
using Pancake.UI;
using UnityEditor;
using UnityEngine;

public class HPopupChooseSeed : UIPopup
{
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();
    [SerializeField] private ChooseSeedBtn chooseSeedBtnPrefab;
    [SerializeField] private Transform buttonParent;

    private void Start()
    {
        foreach (var resourceConfig in resourceConfigList)
        {
            ChooseSeedBtn chooseSeedBtn = Instantiate(chooseSeedBtnPrefab, buttonParent);
            chooseSeedBtn.Initialize(resourceConfig);
        }
    }

    protected override bool EnableTrackBackButton()
    {
        return false;
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
