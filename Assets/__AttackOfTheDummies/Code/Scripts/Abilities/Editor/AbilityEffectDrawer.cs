using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IAbilityEffect), true)]
public class AbilityEffectDrawer : PropertyDrawer
{
    private static Type[] resultTypes;
    private static string[] resultTypeNames;

    static AbilityEffectDrawer()
    {
        resultTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAbilityEffect).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToArray();

        resultTypeNames = resultTypes.Select(type => type.Name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.managedReferenceValue == null)
        {
            int selectedIndex = EditorGUI.Popup(position, "Effect Type", -1, resultTypeNames);
            if (selectedIndex >= 0)
            {
                property.managedReferenceValue = Activator.CreateInstance(resultTypes[selectedIndex]);
                property.isExpanded = true; // Expand newly added result
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }
        else
        {
            // Show script name instead of "Element X"
            label.text = property.managedReferenceValue.GetType().Name;

            // Use PropertyField (which has its own foldout) and ensure it's expanded
            property.isExpanded = true;
            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}