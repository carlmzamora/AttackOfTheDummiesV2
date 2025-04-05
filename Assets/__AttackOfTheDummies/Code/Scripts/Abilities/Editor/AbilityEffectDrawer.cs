using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IAbilityEffect), true)]
public class AbilityEffectDrawer : PropertyDrawer
{
    private static Type[] effectTypes;
    private static string[] effectTypeNames;

    static AbilityEffectDrawer()
    {
        effectTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IAbilityEffect).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();

        effectTypeNames = effectTypes.Select(t => t.Name).ToArray();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.managedReferenceValue == null)
        {
            int selectedIndex = EditorGUI.Popup(position, "Effect Type", -1, effectTypeNames);
            if (selectedIndex >= 0)
            {
                property.managedReferenceValue = Activator.CreateInstance(effectTypes[selectedIndex]);
                property.isExpanded = true;
                property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            EditorGUI.EndProperty();
            return;
        }

        label.text = property.managedReferenceValue.GetType().Name.Replace("Effect", "");

        // Draw header label
        Rect labelRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);

        // Move down below the label
        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var obj = property.managedReferenceValue;
        var type = obj.GetType();

        bool hideAffectRule = fieldInfo.GetCustomAttribute<HideAffectRuleAttribute>() != null;

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (hideAffectRule && field.Name == "affectRule")
                continue;

            var childProperty = property.FindPropertyRelative(field.Name);
            if (childProperty == null) continue;

            float height = EditorGUI.GetPropertyHeight(childProperty, true);
            Rect fieldRect = new(position.x, position.y, position.width, height);
            EditorGUI.PropertyField(fieldRect, childProperty, true);
            position.y += height + EditorGUIUtility.standardVerticalSpacing;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.managedReferenceValue == null)
            return EditorGUIUtility.singleLineHeight;

        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        var obj = property.managedReferenceValue;
        var type = obj.GetType();

        bool hideAffectRule = fieldInfo.GetCustomAttribute<HideAffectRuleAttribute>() != null;

        foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            if (hideAffectRule && field.Name == "affectRule")
                continue;

            var childProperty = property.FindPropertyRelative(field.Name);
            if (childProperty == null) continue;

            height += EditorGUI.GetPropertyHeight(childProperty, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return height;
    }
}