using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ShowIfAttribute))]
public class ShowIfDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!ShouldShow(property))
            return 0f;

        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (ShouldShow(property))
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    private bool ShouldShow(SerializedProperty property)
    {
        ShowIfAttribute showIf = (ShowIfAttribute)attribute;
        SerializedProperty conditionProp = FindConditionProperty(property, showIf.conditionFieldName);

        if (conditionProp == null)
        {
            Debug.LogWarning($"ShowIf: Could not find condition field '{showIf.conditionFieldName}'");
            return true;
        }

        switch (conditionProp.propertyType)
        {
            case SerializedPropertyType.Boolean:
                return conditionProp.boolValue == showIf.expectedValue;
            default:
                Debug.LogWarning($"ShowIf supports only bool fields. Field '{showIf.conditionFieldName}' is of type {conditionProp.propertyType}.");
                return true;
        }
    }

    private SerializedProperty FindConditionProperty(SerializedProperty property, string conditionFieldName)
    {
        string path = property.propertyPath;
        string parentPath = path.Contains(".") ? path.Substring(0, path.LastIndexOf('.')) : "";
        return property.serializedObject.FindProperty(string.IsNullOrEmpty(parentPath)
            ? conditionFieldName
            : $"{parentPath}.{conditionFieldName}");
    }
}