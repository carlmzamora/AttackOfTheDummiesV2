using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AffectMask))]
public class AffectTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Get the FactionDatabase instance
        FactionDatabase database = AssetDatabase.LoadAssetAtPath<FactionDatabase>("Assets/__AttackOfTheDummies/FactionDatabase.asset");
        if (database == null)
        {
            EditorGUI.LabelField(position, label.text, "FactionDatabase not found!");
            return;
        }

        string[] affectTypes = database.GetAffectTypes();
        if (affectTypes.Length == 0)
        {
            EditorGUI.LabelField(position, label.text, "No AffectTypes registered!");
            return;
        }

        // Retrieve the mask value
        SerializedProperty maskProperty = property.FindPropertyRelative("mask");
        int currentMask = maskProperty.intValue;

        // Draw the dropdown
        int newMask = EditorGUI.MaskField(position, label, currentMask, affectTypes);

        // Apply changes if modified
        if (newMask != currentMask)
        {
            maskProperty.intValue = newMask;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}