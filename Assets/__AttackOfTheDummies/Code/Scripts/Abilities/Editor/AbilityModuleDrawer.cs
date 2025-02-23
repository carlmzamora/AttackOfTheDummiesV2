using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IAbilityModule), true)]
public class AbilityModuleDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;

        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 6;

        SerializedProperty iterator = property.Copy();
        if (iterator.Next(true))
        {
            do
            {
                height += EditorGUI.GetPropertyHeight(iterator, true) + EditorGUIUtility.standardVerticalSpacing;
            } while (iterator.Next(false));
        }

        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect boxRect = new Rect(position.x, position.y, position.width, position.height - EditorGUIUtility.standardVerticalSpacing + 8);
        EditorGUI.HelpBox(boxRect, "", MessageType.None);

        // Button to switch the module
        Rect buttonRect = new Rect(position.x + 4f, position.y + 4f, position.width - 8f, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(buttonRect, property.managedReferenceValue != null ? "Switch Ability Module" : "Select Ability Module"))
        {
            ShowModuleSelectionMenu(property);
        }

        // Add space after the button
        float moduleY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 4;

        // Draw the module if it exists
        if (property.managedReferenceValue != null)
        {
            SerializedProperty iterator = property.Copy();

            if (iterator.Next(true))
            {
                do
                {
                    float height = EditorGUI.GetPropertyHeight(iterator, true);

                    // Indent lists slightly for better clarity
                    float indent = iterator.isArray ? 16f : 4f;
                    Rect fieldRect = new Rect(position.x + indent, moduleY, position.width - (indent * 2), height);

                    EditorGUI.PropertyField(fieldRect, iterator, true);

                    moduleY += height + EditorGUIUtility.standardVerticalSpacing;

                } while (iterator.Next(false));
            }
        }

        EditorGUI.EndProperty();
    }

    private void ShowModuleSelectionMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();

        Type[] moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAbilityModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .ToArray();

        foreach (Type moduleType in moduleTypes)
        {
            menu.AddItem(new GUIContent(moduleType.Name), false, () => SetModule(property, moduleType));
        }

        menu.ShowAsContext();
    }

    private void SetModule(SerializedProperty property, Type moduleType)
    {
        // Clear the old module
        property.managedReferenceValue = null;

        // Create and assign a new module
        object newModule = Activator.CreateInstance(moduleType);
        property.managedReferenceValue = newModule;

        property.serializedObject.ApplyModifiedProperties();
    }
}
