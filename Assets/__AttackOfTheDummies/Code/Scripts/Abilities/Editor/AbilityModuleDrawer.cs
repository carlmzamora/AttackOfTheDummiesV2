using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IAbilityModule), true)]
public class AbilityModuleDrawer : PropertyDrawer
{
    private GUIStyle popupStyle;

    private static Dictionary<string, List<Type>> categorizedModules;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing * 2;

        float height = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;

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

        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };
        }

        EnsureModuleCache();

        // Dropdown for selecting the module
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, "Ability Module");

        Rect dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2f, position.y, position.width - EditorGUIUtility.labelWidth - 24f, EditorGUIUtility.singleLineHeight);
        DrawModuleDropdown(property, dropdownRect);

        // Settings button with three-dot icon
        Rect settingsButtonRect = new Rect(position.x + position.width - 20f, position.y, 28f, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(settingsButtonRect, "", popupStyle))
        {
            ShowSettingsMenu(property);
        }

        if (property.managedReferenceValue != null)
        {
            float boxY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Draw the box around module fields
            Rect boxRect = new Rect(position.x, boxY, position.width, position.height - boxY + position.y);
            EditorGUI.HelpBox(boxRect, "", MessageType.None);

            float moduleY = boxY + 10f;
            SerializedProperty iterator = property.Copy();
            EditorGUI.indentLevel++;

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

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void DrawModuleDropdown(SerializedProperty property, Rect position)
    {
        int currentIndex = GetCurrentModuleIndex(property);

        if (EditorGUI.DropdownButton(position, new GUIContent(GetCurrentModuleName(property)), FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();

            foreach (var category in categorizedModules)
            {
                foreach (var moduleType in category.Value)
                {
                    string menuLabel = $"{category.Key}/{moduleType.Name.Replace("Module", "")}";
                    bool isSelected = property.managedReferenceValue != null && property.managedReferenceValue.GetType() == moduleType;

                    menu.AddItem(new GUIContent(menuLabel), isSelected, () => SetModule(property, moduleType));
                }
            }

            menu.ShowAsContext();
        }
    }

    private void SetModule(SerializedProperty property, Type moduleType)
    {
        // Clear the old module
        property.managedReferenceValue = null;

        // Create and assign a new module
        object newModule = Activator.CreateInstance(moduleType);

        /*// Use tracker to restore or create the module
        object newModule = AbilityModuleTracker.RestoreOrCreate(moduleType);*/
        property.managedReferenceValue = newModule;

        property.serializedObject.ApplyModifiedProperties();
    }

    private void ShowSettingsMenu(SerializedProperty property)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Select Script"), false, () => HighlightModuleScript(property.managedReferenceValue.GetType()));

        menu.ShowAsContext();
    }

    private void HighlightModuleScript(Type moduleType)
    {
        string[] guids = AssetDatabase.FindAssets($"t:MonoScript {moduleType.Name}");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

            if (script != null && script.GetClass() == moduleType)
            {
                EditorGUIUtility.PingObject(script);
                break;
            }
        }
    }

    private void EnsureModuleCache()
    {
        if (categorizedModules == null)
        {
            categorizedModules = new Dictionary<string, List<Type>>();

            Type[] allModules = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IAbilityModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && type != typeof(AbilityModule))
                .ToArray();

            foreach (var module in allModules)
            {
                string category = GetCategory(module);
                if (!categorizedModules.ContainsKey(category))
                {
                    categorizedModules[category] = new List<Type>();
                }

                categorizedModules[category].Add(module);
            }
        }
    }

    private string GetCategory(Type moduleType)
    {
        if (typeof(IInstantCastModule).IsAssignableFrom(moduleType)) return "Instant Cast";
        if (typeof(ITargetedCastModule).IsAssignableFrom(moduleType)) return "Targeted Cast";
        return "Other";
    }

    private int GetCurrentModuleIndex(SerializedProperty property)
    {
        if (property.managedReferenceValue == null) return -1;

        Type currentType = property.managedReferenceValue.GetType();

        int index = 0;
        foreach (var category in categorizedModules.Values)
        {
            int subIndex = category.IndexOf(currentType);
            if (subIndex != -1) return index + subIndex;

            index += category.Count;
        }

        return -1;
    }

    private string GetCurrentModuleName(SerializedProperty property)
    {
        return property.managedReferenceValue != null ? property.managedReferenceValue.GetType().Name : "None";
    }
}
