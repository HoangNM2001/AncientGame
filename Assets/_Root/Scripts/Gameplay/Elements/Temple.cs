using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;

public class Temple : SaveDataElement
{
    [SerializeField] private List<ResourceConfig> resourceConfigList;
    [SerializeField] private ScriptableListExtendField extendFieldList;
    [SerializeField] private Animator templeManAnimator;
    [SerializeField] private Transform buyAbleParent;
    [SerializeField] private ShowableUI showableUI;
    [SerializeField] private BuyableResource buyAbleResourcePrefab;

    private void Awake()
    {
        Initialize();
    }

    [ContextMenu("Bless")]
    public void Bless()
    {
        foreach (var extendField in extendFieldList)
        {
            if (extendField.ResourceConfig != null)
            {
                foreach (var field in extendField.FieldList)
                {
                    field.ForceGrow();
                }

                extendField.InitCount();
                extendField.OnStateChange?.Invoke();
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Get Resources")]
    public void GetResources()
    {
        const string resourcesFolderPath = "Assets/_Root/ScriptableData/ResourceConfigs";

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
