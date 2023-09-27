using System.Collections;
using System.Collections.Generic;
using Pancake;
using UnityEditor;
using UnityEngine;

public class ExtendField : MonoBehaviour
{
    [SerializeField, ID] private string fieldID;
    [SerializeField] private List<Field> fieldList = new List<Field>();
    [SerializeField] private List<ResourceConfig> resourceConfigList = new List<ResourceConfig>();

    private EnumPack.ResourceType resourceType;
    
    
    
#if UNITY_EDITOR
    [ContextMenu("ResetId")]
    public void ResetId()
    {
        fieldID = IDAttributeDrawer.ToSnakeCase(name);
        EditorUtility.SetDirty(this);
    }
#endif
}
