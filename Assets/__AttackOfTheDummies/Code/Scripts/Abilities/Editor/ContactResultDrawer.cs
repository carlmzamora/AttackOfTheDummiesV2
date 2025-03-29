using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Playables;
using UnityEngine;

[CustomPropertyDrawer(typeof(IContactResult), true)]
public class ContactResultDrawer : PropertyDrawer
{
    private static Type[] resultTypes;
    private static string[] resultTypeNames;
    private static Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();

    static ContactResultDrawer()
    {
        resultTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IContactResult).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToArray();

        resultTypeNames = resultTypes.Select(type => type.Name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.managedReferenceValue == null)
        {
            int selectedIndex = EditorGUI.Popup(position, "Result Type", -1, resultTypeNames);
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