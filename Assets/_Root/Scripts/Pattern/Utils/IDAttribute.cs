#if UNITY_EDITOR
using System.Text;
using UnityEditor;
using UnityEngine;

public class IDAttribute : PropertyAttribute
{
    public IDAttribute()
    {
        
    }
}

[CustomPropertyDrawer(typeof(IDAttribute))]
public class IDAttributeDrawer : PropertyDrawer
{ 
    IDAttribute IDAttribute => attribute as IDAttribute;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Context(position, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        var id = property.stringValue;
        if (string.IsNullOrEmpty(id))
        {
            id = ToSnakeCase(property.serializedObject.targetObject.name);
            property.stringValue = id;
            property.serializedObject.ApplyModifiedProperties();
        }

        GUI.enabled = false;
        EditorGUI.TextField(position, id);

        EditorGUI.EndProperty();
    }

    private static void Context(Rect rect, SerializedProperty property)
    {
        var current = Event.current;

        if (!rect.Contains(current.mousePosition) || current.type != EventType.ContextClick) return;
        var menu = new GenericMenu();
            
        menu.AddItem(new GUIContent("ResetID"), false, () =>
        {
            property.stringValue = ToSnakeCase(property.serializedObject.targetObject.name);
            property.serializedObject.ApplyModifiedProperties();
        });
            
        menu.ShowAsContext();
            
        current.Use();
    }
    
    public static string ToSnakeCase(string text)
    {
        if (text.Length < 2)
        {
            return text;
        }

        var sb = new StringBuilder();
        sb.Append(char.ToLowerInvariant(text[0]));
        for (var i = 1; i < text.Length; ++i)
        {
            var c = text[i];
            if (char.IsUpper(c))
            {
                sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else if (c == ' ' || c == '-')
            {
                continue;
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
#endif
