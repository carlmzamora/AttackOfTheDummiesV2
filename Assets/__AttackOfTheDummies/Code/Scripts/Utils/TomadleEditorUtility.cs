using UnityEditor;
using UnityEngine;
using System.IO;

public static class TomadleEditorUtility
{
    #region FACTIONS

    private const string DatabasePath = "Assets/__AttackOfTheDummies/FactionDatabase.asset";

    [MenuItem("Tomadle/Factions/Create", priority = 1)]
    private static void CreateFactionDatabase()
    {
        if (FactionDatabaseExists())
        {
            SelectFactionDatabase();
            return;
        }

        FactionDatabase database = ScriptableObject.CreateInstance<FactionDatabase>();

        string directory = Path.GetDirectoryName(DatabasePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        AssetDatabase.CreateAsset(database, DatabasePath);
        AssetDatabase.SaveAssets();

        Debug.Log("FactionDatabase created at: " + DatabasePath);
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = database;
    }

    [MenuItem("Tomadle/Factions/Create", true)]
    private static bool ValidateCreateMenuItem()
    {
        return !FactionDatabaseExists();
    }

    [MenuItem("Tomadle/Factions/Select Database", priority = 2)]
    private static void SelectFactionDatabase()
    {
        FactionDatabase database = AssetDatabase.LoadAssetAtPath<FactionDatabase>(DatabasePath);
        if (database != null)
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = database;
        }
        else
        {
            Debug.LogWarning("FactionDatabase not found! Try creating it first.");
        }
    }

    [MenuItem("Tomadle/Factions/Select Database", true)]
    private static bool ValidateSelectMenuItem()
    {
        return FactionDatabaseExists();
    }

    private static bool FactionDatabaseExists()
    {
        return AssetDatabase.LoadAssetAtPath<FactionDatabase>(DatabasePath) != null;
    }

    #endregion
}