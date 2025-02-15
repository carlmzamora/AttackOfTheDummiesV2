using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Modifier), true)]
public class ModifierPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        if (property.managedReferenceValue == null)
        {
            if (GUI.Button(dropdownRect, "Select Modifier"))
            {
                ShowModifierSelectionMenu(property);
            }
        }
        else
        {
            string foldoutKey = property.propertyPath + "_foldout";
            bool isExpanded = EditorPrefs.GetBool(foldoutKey, true);

            // Draw foldout
            isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                                           isExpanded, property.managedReferenceValue.GetType().Name, true);

            // Save foldout state
            EditorPrefs.SetBool(foldoutKey, isExpanded);

            float yOffset = dropdownRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            if (isExpanded)
            {
                // Draw "General" section
                Rect labelRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(labelRect, "General", EditorStyles.boldLabel);
                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                DisplayFields(position, property, GetBaseFieldsToPreview(property), ref yOffset);
                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw derived class properties
                List<FieldInfo> derivedFields = GetDerivedFieldsToPreview(property);
                if (derivedFields.Count > 0)
                {
                    labelRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                    EditorGUI.LabelField(labelRect, property.managedReferenceValue.GetType().Name + " Properties", EditorStyles.boldLabel);
                    yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    DisplayFields(position, property, derivedFields, ref yOffset);
                }
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        string foldoutKey = property.propertyPath + "_foldout";
        bool isExpanded = EditorPrefs.GetBool(foldoutKey, true);

        float height = EditorGUIUtility.singleLineHeight;

        if (isExpanded)
        {
            List<FieldInfo> fieldsToPreview = GetBaseFieldsToPreview(property);
            fieldsToPreview.AddRange(GetDerivedFieldsToPreview(property));

            // Add height dynamically based on property size
            foreach (FieldInfo field in fieldsToPreview)
            {
                SerializedProperty fieldProperty = property.FindPropertyRelative(field.Name);
                if (fieldProperty != null)
                {
                    height += EditorGUI.GetPropertyHeight(fieldProperty, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }

            // Add height for section labels
            height += (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }

        height += 10f;

        return height;
    }

    private List<FieldInfo> GetBaseFieldsToPreview(SerializedProperty property)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return property.managedReferenceValue.GetType().BaseType.GetFields(flags)
            .Where(field => !Attribute.IsDefined(field, typeof(HideInInspector)))
            .ToList();
    }

    private List<FieldInfo> GetDerivedFieldsToPreview(SerializedProperty property)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        return property.managedReferenceValue.GetType().GetFields(flags)
            .Where(field => !Attribute.IsDefined(field, typeof(HideInInspector)))
            .ToList();
    }

    private void DisplayFields(Rect position, SerializedProperty property, List<FieldInfo> fields, ref float yOffset)
    {
        //bool shouldShowFactionMask = property.propertyPath.Contains("modifiersAppliedInRadiusOnCast");

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(FactionMask) && DoesParentListHideFactionMask(property))
            {
                continue; // Skip drawing FactionMask if not in modifiersAppliedInRadiusOnCast
            }

            SerializedProperty fieldProperty = property.FindPropertyRelative(field.Name);
            if (fieldProperty != null)
            {
                // Extract tooltip from TooltipAttribute
                TooltipAttribute tooltip = field.GetCustomAttribute<TooltipAttribute>();
                string tooltipText = tooltip != null ? tooltip.tooltip : "";

                // Create label with tooltip
                GUIContent fieldLabel = new GUIContent(ObjectNames.NicifyVariableName(field.Name), tooltipText);

                Rect fieldRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(fieldRect, fieldProperty, fieldLabel, true);

                // Add standard spacing after each field
                yOffset += EditorGUI.GetPropertyHeight(fieldProperty, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }

    private void ShowModifierSelectionMenu(SerializedProperty property)
    {
        List<Type> modifierTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Modifier)))
            .ToList();

        GenericMenu menu = new GenericMenu();
        foreach (Type type in modifierTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => AssignModifier(property, type));
        }
        menu.ShowAsContext();
    }

    private void AssignModifier(SerializedProperty property, Type type)
    {
        object instance = Activator.CreateInstance(type);
        property.serializedObject.Update();
        property.managedReferenceValue = instance;
        property.serializedObject.ApplyModifiedProperties();
    }

    private bool DoesParentListHideFactionMask(SerializedProperty property)
    {
        FieldInfo field = GetParentField(property);
        return field != null && field.IsDefined(typeof(HideFactionMaskAttribute), false);
    }

    private FieldInfo GetParentField(SerializedProperty property)
    {
        Type parentType = property.serializedObject.targetObject.GetType();
        FieldInfo[] fields = parentType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(List<Modifier>))
            {
                SerializedProperty listProperty = property.serializedObject.FindProperty(field.Name);
                if (listProperty != null && property.propertyPath.Contains(listProperty.propertyPath))
                    return field;
            }
        }
        return null;
    }
}