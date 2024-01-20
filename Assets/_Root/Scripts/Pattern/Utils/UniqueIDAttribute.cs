#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class UniqueIDAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(UniqueIDAttribute))]
public class UniqueIdentifierDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        // Place a label so it can't be edited by accident
        Rect textFieldPosition = position;
        textFieldPosition.height = 16;
        DrawLabelField(textFieldPosition, prop, label);
    }

    void DrawLabelField(Rect position, SerializedProperty prop, GUIContent label)
    {
        EditorGUI.LabelField(position, label, new GUIContent(prop.stringValue));
    }
}
#endif