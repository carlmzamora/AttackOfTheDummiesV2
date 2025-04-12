using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IAbilityEffect), true)]
public class AbilityEffectDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect dropdownRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        if (property.managedReferenceValue == null)
        {
            if (GUI.Button(dropdownRect, "Select Effect Type"))
            {
                ShowEffectSelectionMenu(property);
            }
        }
        else
        {
            string foldoutKey = property.propertyPath + "_foldout";
            bool isExpanded = EditorPrefs.GetBool(foldoutKey, true);

            isExpanded = EditorGUI.Foldout(dropdownRect, isExpanded, property.managedReferenceValue.GetType().Name, true);
            EditorPrefs.SetBool(foldoutKey, isExpanded);

            float yOffset = dropdownRect.yMax + EditorGUIUtility.standardVerticalSpacing;

            if (isExpanded)
            {
                DisplayFields(position, property, GetBaseFieldsToPreview(property), ref yOffset);
                DisplayFields(position, property, GetDerivedFieldsToPreview(property), ref yOffset);
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
            List<FieldInfo> fields = GetBaseFieldsToPreview(property);
            fields.AddRange(GetDerivedFieldsToPreview(property));

            foreach (FieldInfo field in fields)
            {
                SerializedProperty fieldProperty = property.FindPropertyRelative(field.Name);
                if (fieldProperty != null)
                {
                    height += EditorGUI.GetPropertyHeight(fieldProperty, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
        }

        height += 10f;
        return height;
    }

    private List<FieldInfo> GetBaseFieldsToPreview(SerializedProperty property)
    {
        var type = property.managedReferenceValue.GetType().BaseType;
        return type?.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
               .Where(field => !Attribute.IsDefined(field, typeof(HideInInspector)))
               .ToList() ?? new();
    }

    private List<FieldInfo> GetDerivedFieldsToPreview(SerializedProperty property)
    {
        var type = property.managedReferenceValue.GetType();
        return type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
               .Where(field => !Attribute.IsDefined(field, typeof(HideInInspector)))
               .ToList();
    }

    private void DisplayFields(Rect position, SerializedProperty property, List<FieldInfo> fields, ref float yOffset)
    {
        foreach (FieldInfo field in fields)
        {
            SerializedProperty fieldProperty = property.FindPropertyRelative(field.Name);
            if (fieldProperty == null)
                continue;

            TooltipAttribute tooltip = field.GetCustomAttribute<TooltipAttribute>();
            string tooltipText = tooltip != null ? tooltip.tooltip : "";

            GUIContent fieldLabel = new GUIContent(ObjectNames.NicifyVariableName(field.Name), tooltipText);
            Rect fieldRect = new Rect(position.x, yOffset, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(fieldRect, fieldProperty, fieldLabel, true);

            yOffset += EditorGUI.GetPropertyHeight(fieldProperty, true) + EditorGUIUtility.standardVerticalSpacing;
        }
    }

    private void ShowEffectSelectionMenu(SerializedProperty property)
    {
        List<Type> effectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IAbilityEffect).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToList();

        GenericMenu menu = new GenericMenu();
        foreach (Type type in effectTypes)
        {
            menu.AddItem(new GUIContent(type.Name), false, () => AssignEffect(property, type));
        }
        menu.ShowAsContext();
    }

    private void AssignEffect(SerializedProperty property, Type type)
    {
        object instance = Activator.CreateInstance(type);
        property.serializedObject.Update();
        property.managedReferenceValue = instance;
        property.serializedObject.ApplyModifiedProperties();
    }
}