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

    private GUIStyle popupStyle;
    private Dictionary<string, List<MonoScript>> categorizedModules;

    private void OnEnable()
    {
        ability = (ActiveAbility)target;
        abilityModuleScripts = GetAbilityModuleScripts();
        CategorizeModules();
        ActiveAbilityModuleFixer.ValidateModule(ability);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (popupStyle == null)
        {
            popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"))
            {
                imagePosition = ImagePosition.ImageOnly
            };
        }

        // Draw base Ability fields
        DrawBaseAbilityFields();
        EditorGUILayout.Space(10);

        // Draw the "Ability Module" label and dropdown
        Rect labelRect = GUILayoutUtility.GetRect(EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, "Ability Module");

        Rect dropdownRect = new Rect(labelRect.x + EditorGUIUtility.labelWidth + 2f, labelRect.y,
                                     EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - 40f,
                                     EditorGUIUtility.singleLineHeight);

        DrawModuleDropdown(dropdownRect);

        Rect settingsButtonRect = new Rect(dropdownRect.x + dropdownRect.width, dropdownRect.y, 28f, EditorGUIUtility.singleLineHeight);
        if (GUI.Button(settingsButtonRect, "", popupStyle))
        {
            ShowSettingsMenu(ability.abilityModule);
        }

        if (ability.abilityModule != null && ability.hasChosenModule)
        {
            SerializedProperty abilityModuleProp = serializedObject.FindProperty("abilityModule");
            List<FieldInfo> moduleFields = GetFieldsToPreview(abilityModuleProp);

            // HelpBox and Module Content as one block
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.Space(10);

            EditorGUI.indentLevel++;

            DisplayFields(abilityModuleProp, moduleFields);
            EditorGUILayout.Space(10);
            GUILayout.EndVertical();
        }

        EditorGUI.indentLevel++;

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

    #region MODULE SELECTION

    private void DrawModuleDropdown(Rect position)
    {
        string currentModuleName = ability.moduleScript ? ability.moduleScript.name : "None";

        // Dropdown button
        if (EditorGUI.DropdownButton(position, new GUIContent(currentModuleName), FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();

            foreach (var category in categorizedModules)
            {
                foreach (var script in category.Value)
                {
                    bool isSelected = script == ability.moduleScript;
                    menu.AddItem(new GUIContent($"{category.Key}/{script.name.Replace("Module", "")}"), isSelected, () =>
                    {
                        ability.moduleScript = script;
                        ability.InstantiateModule();
                        ActiveAbilityModuleFixer.SaveModuleData(ability);
                        EditorUtility.SetDirty(ability);
                    });
                }
            }

            menu.ShowAsContext();
        }
    }

    private void CategorizeModules()
    {
        categorizedModules = new Dictionary<string, List<MonoScript>>();

        foreach (var script in abilityModuleScripts)
        {
            string category = GetCategory(script.GetClass());

            if (!categorizedModules.ContainsKey(category))
            {
                categorizedModules[category] = new List<MonoScript>();
            }

            categorizedModules[category].Add(script);
        }
    }

    private string GetCategory(Type moduleType)
    {
        if (typeof(IInstantCastModule).IsAssignableFrom(moduleType)) return "Instant Cast";
        if (typeof(ITargetedCastModule).IsAssignableFrom(moduleType)) return "Targeted Cast";
        return "Other";
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
                return scriptType != null && typeof(AbilityModule).IsAssignableFrom(scriptType) && !scriptType.IsAbstract && scriptType != typeof(AbilityModule);
            })
            .ToList();
    }

    #endregion

    #region MODULE DISPLAY

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

                if (GUI.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                    ActiveAbilityModuleFixer.SaveModuleData(ability);
                }
            }
        }
    }

    #endregion

    #region SETTINGS

    private void ShowSettingsMenu(IAbilityModule module)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Select Script"), false, () => HighlightModuleScript(module.GetType()));

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

    #endregion
}

// Automatically fixes ActiveAbility module references on script rename
public class ActiveAbilityModuleFixer : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
    {
        if (movedAssets.Length == 0) return;

        var allAbilities = AssetDatabase.FindAssets("t:ActiveAbility")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<ActiveAbility>);

        foreach (var ability in allAbilities)
        {
            ValidateModule(ability);
        }
    }

    public static void ValidateModule(ActiveAbility ability)
    {
        if (ability == null || ability.moduleScript == null || ability.abilityModule != null) return;

        if(ability.abilityModule == null && ability.hasChosenModule)
        {
            ability.InstantiateModule();

            if (!string.IsNullOrEmpty(ability.moduleDataJson) && ability.abilityModule != null)
            {
                JsonUtility.FromJsonOverwrite(ability.moduleDataJson, ability.abilityModule);
                EditorUtility.SetDirty(ability);

                // Clear missing references
                SerializationUtility.ClearAllManagedReferencesWithMissingTypes(ability);

                // Log the fix
                Debug.Log($"Fixed missing references in ActiveAbility ScriptableObject '{ability.name}'.", ability);
            }
        }
    }

    public static void SaveModuleData(ActiveAbility ability)
    {
        if (ability == null || ability.abilityModule == null) return;
        ability.moduleDataJson = JsonUtility.ToJson(ability.abilityModule);
    }
}
