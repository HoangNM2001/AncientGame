using System.Collections;
using System.Collections.Generic;
using System.Text;
using Pancake;
using Pancake.Apex;
using Pancake.Scriptable;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

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
    public GameObjectPool flyUIPool;
    public IntVariable resourceQuantity;
    public int price;
    public float exp;

#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        resourceId = IDAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
    }
#endif
}