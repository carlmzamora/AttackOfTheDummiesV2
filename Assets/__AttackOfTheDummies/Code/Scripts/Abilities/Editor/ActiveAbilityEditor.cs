using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

[CustomEditor(typeof(ActiveAbility))]
public class ActiveAbilityEditor : Editor
{
    private ActiveAbility ability;
    private List<MonoScript> abilityModuleScripts;

    private void OnEnable()
    {
        ability = (ActiveAbility)target;
        abilityModuleScripts = GetAbilityModuleScripts();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw base Ability fields
        DrawBaseAbilityFields();

        // Draw the "Ability Module" label and dropdown
        Rect labelRect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, "Ability Module");

        Rect dropdownRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth + 2f, labelRect.y,
                                     EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 40f,
                                     EditorGUIUtility.singleLineHeight);

        DrawModuleDropdown(dropdownRect);

        // Display Ability Module fields
        if (ability.abilityModule != null)
        {
            float yOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty abilityModuleProp = serializedObject.FindProperty("abilityModule");
            List<FieldInfo> moduleFields = GetFieldsToPreview(abilityModuleProp);

            DisplayFields(abilityModuleProp, moduleFields);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawBaseAbilityFields()
    {
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;

        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            if (property.propertyPath == "moduleScript" || property.propertyPath == "abilityModule")
                continue; // Skip module-related properties (handled separately)

            EditorGUILayout.PropertyField(property, true);
        }
    }

    private void DrawModuleDropdown(Rect position)
    {
        string currentModuleName = ability.moduleScript ? ability.moduleScript.name : "None";

        // Dropdown button
        if (EditorGUI.DropdownButton(position, new GUIContent(currentModuleName), FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();

            foreach (var script in abilityModuleScripts)
            {
                bool isSelected = script == ability.moduleScript;

                menu.AddItem(new GUIContent(script.name.Replace("Module", "")), isSelected, () =>
                {
                    ability.moduleScript = script;
                    ability.InstantiateModule();
                    EditorUtility.SetDirty(ability);
                });
            }

            menu.ShowAsContext();
        }
    }

    // Collects all MonoScripts inheriting from AbilityModule
    private List<MonoScript> GetAbilityModuleScripts()
    {
        return AssetDatabase.FindAssets("t:MonoScript")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
            .Where(script =>
            {
                Type scriptType = script.GetClass();
                return scriptType != null && typeof(AbilityModule).IsAssignableFrom(scriptType) && !scriptType.IsAbstract;
            })
            .ToList();
    }

    private List<FieldInfo> GetFieldsToPreview(SerializedProperty property)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return property.managedReferenceValue?.GetType().GetFields(flags)
            .Where(field => !Attribute.IsDefined(field, typeof(HideInInspector)))
            .ToList() ?? new List<FieldInfo>();
    }

    private void DisplayFields(SerializedProperty property, List<FieldInfo> fields)
    {
        foreach (FieldInfo field in fields)
        {
            SerializedProperty fieldProperty = property.FindPropertyRelative(field.Name);
            if (fieldProperty != null)
            {
                // Extract tooltip from TooltipAttribute
                TooltipAttribute tooltip = field.GetCustomAttribute<TooltipAttribute>();
                string tooltipText = tooltip != null ? tooltip.tooltip : "";

                // Create label with tooltip
                GUIContent fieldLabel = new GUIContent(ObjectNames.NicifyVariableName(field.Name), tooltipText);

                EditorGUILayout.PropertyField(fieldProperty, fieldLabel, true);
            }
        }
    }
}