using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Modifier), true)]
public class ModifierPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        if(property.managedReferenceValue == null)
        {
            if(GUI.Button(dropdownRect, "Select Modifier"))
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

            if(isExpanded)
            {
                Rect labelRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.LabelField(labelRect, "General", EditorStyles.boldLabel);
                yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                DisplayFields(position, property, label, GetBaseFieldsToPreview(property), ref yOffset);

                List<FieldInfo> derivedFields = GetDerivedFieldsToPreview(property);
                if(derivedFields.Count > 0)
                {
                    labelRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);

                    EditorGUI.LabelField(labelRect, property.managedReferenceValue.GetType().Name + " Properties", EditorStyles.boldLabel);
                    yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                    DisplayFields(position, property, label, derivedFields, ref yOffset);
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

        if(isExpanded)
        {
            List<FieldInfo> fieldsToPreview = GetBaseFieldsToPreview(property);
            fieldsToPreview.AddRange(GetDerivedFieldsToPreview(property));

            foreach (FieldInfo field in fieldsToPreview)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            //add two lines for the section labels
            height += EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        return height;
    }

    public List<FieldInfo> GetBaseFieldsToPreview(SerializedProperty property)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        List<FieldInfo> baseFields = property.managedReferenceValue.GetType().BaseType.GetFields(flags)
                .Where(field => Attribute.IsDefined(field, typeof(PreviewInDrawerAttribute)))
                .ToList();

        return baseFields;
    }

    public List<FieldInfo> GetDerivedFieldsToPreview(SerializedProperty property)
    {
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        List<FieldInfo> derivedFields = property.managedReferenceValue.GetType().GetFields(flags)
                .Where(field => Attribute.IsDefined(field, typeof(PreviewInDrawerAttribute)))
                .ToList();

        return derivedFields;
    }

    public void DisplayFields(Rect position, SerializedProperty property, GUIContent label, List<FieldInfo> fields, ref float yOffset)
    {
        foreach (FieldInfo field in fields)
        {
            object fieldValue = field.GetValue(property.managedReferenceValue);
            Rect fieldRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            if (fieldValue is float floatValue)
            {
                float newValue = EditorGUI.FloatField(fieldRect, ObjectNames.NicifyVariableName(field.Name), floatValue);
                if (!Mathf.Approximately(floatValue, newValue))
                {
                    field.SetValue(property.managedReferenceValue, newValue);
                }
            }
            else if (fieldValue is int intValue)
            {
                int newValue = EditorGUI.IntField(fieldRect, ObjectNames.NicifyVariableName(field.Name), intValue);
                if (intValue != newValue)
                {
                    field.SetValue(property.managedReferenceValue, newValue);
                }
            }
            else if (fieldValue is string stringValue)
            {
                string newValue = EditorGUI.TextField(fieldRect, ObjectNames.NicifyVariableName(field.Name), stringValue);
                if (stringValue != newValue)
                {
                    field.SetValue(property.managedReferenceValue, newValue);
                }
            }
            yOffset += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private void ShowModifierSelectionMenu(SerializedProperty property)
    {
        List<Type> modifierTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Modifier)))
                .ToList();

        GenericMenu menu = new GenericMenu();
        foreach(Type type in modifierTypes)
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
