using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FactionMask))]
public class FactionMaskDrawer : PropertyDrawer
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

        List<FactionData> factions = database.GetFactions();
        if (factions.Count == 0)
        {
            EditorGUI.LabelField(position, label.text, "No factions registered!");
            return;
        }

        string[] factionNames = new string[factions.Count];
        for (int i = 0; i < factions.Count; i++)
        {
            factionNames[i] = factions[i].factionName;
        }

        // Retrieve the mask value
        SerializedProperty maskProperty = property.FindPropertyRelative("mask");
        int currentMask = maskProperty.intValue;

        // Draw the dropdown
        int newMask = EditorGUI.MaskField(position, label, currentMask, factionNames);

        // Apply changes if modified
        if (newMask != currentMask)
        {
            maskProperty.intValue = newMask;
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}