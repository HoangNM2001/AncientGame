using System.Collections;
using System.Collections.Generic;
using System.Text;
using Pancake;
using Pancake.Apex;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Resources")]
public class ResourceConfig : ScriptableObject
{
    [SerializeField, ID] private string resourceId = "string";
    public EnumPack.ResourceType resourceType;
    public Sprite resourceIcon;
    public GameObjectPool smallTreePool;
    public GameObjectPool bigTreePool;
    public Color treeColor;
    public GameObjectPool flyModelPool;

    public string ResourceId => resourceId;

#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        resourceId = IDAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
    }
#endif
}