using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Faction))]
public class FactionPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, "Faction");

        Rect dropdownRect = new Rect(position.x + EditorGUIUtility.labelWidth + 2f, position.y, position.width - EditorGUIUtility.labelWidth - 2f, EditorGUIUtility.singleLineHeight);

        SerializedProperty indexProp = property.FindPropertyRelative("factionIndex");
        // Get the FactionDatabase instance
        FactionDatabase database = AssetDatabase.LoadAssetAtPath<FactionDatabase>("Assets/__AttackOfTheDummies/FactionDatabase.asset");
        if (database == null)
        {
            EditorGUI.LabelField(position, label.text, "FactionDatabase not found!");
            return;
        }

        var factions = database.GetAllFactionData();
        string currentFactionName = (indexProp.intValue >= 0 && indexProp.intValue < factions.Count)
                                    ? factions[indexProp.intValue].factionName
                                    : "None";

        // Dropdown button
        if (EditorGUI.DropdownButton(dropdownRect, new GUIContent($"{currentFactionName}"), FocusType.Passive))
        {
            GenericMenu menu = new GenericMenu();

            // Populate dropdown options
            for (int i = 0; i < factions.Count; i++)
            {
                int factionIndex = i;
                string factionName = factions[i].factionName;
                bool isSelected = (factionIndex == indexProp.intValue);

                menu.AddItem(new GUIContent(factionName), isSelected, () =>
                {
                    indexProp.intValue = factionIndex;
                    property.serializedObject.ApplyModifiedProperties();
                });
            }

            // Show dropdown
            menu.ShowAsContext();
        }

        EditorGUI.EndProperty();
    }
}