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
                EditorGUI.indentLevel++;
                DisplayFields(position, property, GetBaseFieldsToPreview(property), ref yOffset);

                // Draw derived class properties
                List<FieldInfo> derivedFields = GetDerivedFieldsToPreview(property);
                if (derivedFields.Count > 0)
                {
                    DisplayFields(position, property, derivedFields, ref yOffset);
                }
                EditorGUI.indentLevel--;
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

            // Group and add height based on header foldouts
            Dictionary<string, List<FieldInfo>> headerGroups = new Dictionary<string, List<FieldInfo>>();
            string currentHeader = "";

            foreach (FieldInfo field in fieldsToPreview)
            {
                FoldingHeaderAttribute headerAttr = field.GetCustomAttribute<FoldingHeaderAttribute>();
                if (headerAttr != null)
                    currentHeader = headerAttr.name;

                if (!headerGroups.ContainsKey(currentHeader))
                    headerGroups[currentHeader] = new List<FieldInfo>();

                headerGroups[currentHeader].Add(field);
            }

            foreach (var group in headerGroups)
            {
                string headerKey = property.propertyPath + "_header_" + group.Key;
                bool headerExpanded = EditorPrefs.GetBool(headerKey, string.IsNullOrEmpty(group.Key) ? true : false);

                if (!string.IsNullOrEmpty(group.Key))
                    height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (headerExpanded)
                {
                    foreach (FieldInfo field in group.Value)
                    {
                        SerializedProperty fieldProp = property.FindPropertyRelative(field.Name);
                        if (fieldProp != null)
                        {
                            height += EditorGUI.GetPropertyHeight(fieldProp, true) + EditorGUIUtility.standardVerticalSpacing;
                        }
                    }
                }
            }
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
        Dictionary<string, List<FieldInfo>> headerGroups = new Dictionary<string, List<FieldInfo>>();
        string currentHeader = "";

        foreach (FieldInfo field in fields)
        {
            FoldingHeaderAttribute headerAttr = field.GetCustomAttribute<FoldingHeaderAttribute>();
            if (headerAttr != null)
            {
                currentHeader = headerAttr.name;
            }

            if (!headerGroups.ContainsKey(currentHeader))
                headerGroups[currentHeader] = new List<FieldInfo>();

            headerGroups[currentHeader].Add(field);
        }

        foreach (var group in headerGroups)
        {
            string headerKey = property.propertyPath + "_header_" + group.Key;

            // Default open only if it's not the default group (i.e. "") 
            bool expanded = EditorPrefs.GetBool(headerKey, string.IsNullOrEmpty(group.Key) ? true : false);

            if (!string.IsNullOrEmpty(group.Key))
            {
                Rect headerRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
                expanded = EditorGUI.Foldout(headerRect, expanded, group.Key, true);
                EditorPrefs.SetBool(headerKey, expanded);
                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            if (expanded)
            {
                EditorGUI.indentLevel++;
                foreach (FieldInfo field in group.Value)
                {
                    SerializedProperty fieldProp = property.FindPropertyRelative(field.Name);
                    if (fieldProp != null)
                    {
                        TooltipAttribute tooltip = field.GetCustomAttribute<TooltipAttribute>();
                        string tooltipText = tooltip != null ? tooltip.tooltip : "";
                        GUIContent fieldLabel = new GUIContent(ObjectNames.NicifyVariableName(field.Name), tooltipText);

                        float height = EditorGUI.GetPropertyHeight(fieldProp, true);
                        Rect fieldRect = new Rect(position.x, yOffset, position.width, height);
                        EditorGUI.PropertyField(fieldRect, fieldProp, fieldLabel, true);

                        yOffset += height + EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                EditorGUI.indentLevel--;
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
}