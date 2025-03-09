using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AbilityModulesTracker
{
    public static Dictionary<string, List<Type>> categorizedModules;

    private static readonly string snapshotPath = "Assets/AbilityModulesSnapshot.json";

    public static void CategorizeModules()
    {
        if (categorizedModules == null)
        {
            categorizedModules = new Dictionary<string, List<Type>>();

            Type[] allModules = LoadAllAbilityModules();

            foreach (var module in allModules)
            {
                string category = GetCategory(module);
                if (!categorizedModules.ContainsKey(category))
                {
                    categorizedModules[category] = new List<Type>();
                }

                categorizedModules[category].Add(module);
            }
        }
    }

    private static Type[] LoadAllAbilityModules()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAbilityModule).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract && type != typeof(AbilityModule))
            .ToArray();
    }

    private static ModulesSnapshot LoadSnapshot()
    {
        if (File.Exists(snapshotPath))
        {
            string json = File.ReadAllText(snapshotPath);
            return JsonUtility.FromJson<ModulesSnapshot>(json);
        }
        else
        {
            return new ModulesSnapshot();
        }
    }

    private static void SaveSnapshot(Type[] modules)
    {
        ModulesSnapshot snapshot = new()
        {
            moduleNames = modules.Select(m => m.FullName).ToList(),
            moduleGUIDs = GetModulesGUIDs(modules)
        };

        File.WriteAllText(snapshotPath, JsonUtility.ToJson(snapshot, true));
        Debug.Log("[AbilityModulesTracker] Module snapshot updated.");
    }

    private static List<string> GetModulesGUIDs(Type[] modules)
    {
        List<string> guidsToReturn = new();
        string[] guidsInDatabase = AssetDatabase.FindAssets("t:MonoScript");

        for(int i = 0; i < modules.Length; i++)
        {
            Type currentModule = modules[i];

            for (int j = 0; j < guidsInDatabase.Length; j++)
            {
                string currentGUID = guidsInDatabase[j];

                string path = AssetDatabase.GUIDToAssetPath(currentGUID);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);

                if (script != null && script.GetClass() == currentModule)
                {
                    guidsToReturn.Add(currentGUID);
                    break;
                }
            }
        }

        return guidsToReturn;
    }

    private static string GetCategory(Type moduleType)
    {
        if (typeof(IInstantCastModule).IsAssignableFrom(moduleType)) return "Instant Cast";
        if (typeof(ITargetedCastModule).IsAssignableFrom(moduleType)) return "Targeted Cast";
        return "Other";
    }

    public static void FixManagedReferenceValue(SerializedProperty property)
    {
        Type[] currentModules = LoadAllAbilityModules();
        ModulesSnapshot lastSnapshot = LoadSnapshot();

        var lostModules = currentModules.Where(m => !lastSnapshot.moduleNames.Contains(m.FullName)).ToArray();

        if (lostModules.Length > 1)
            Debug.LogWarning("More than 1 lost modules!");

        object newModule = Activator.CreateInstance(lostModules.FirstOrDefault());
        property.managedReferenceValue = newModule;

        SaveSnapshot(currentModules);
        CategorizeModules();
        SerializationUtility.ClearAllManagedReferencesWithMissingTypes(property.serializedObject.targetObject);
    }

    [Serializable]
    private class ModulesSnapshot
    {
        public List<string> moduleNames = new();
        public List<string> moduleGUIDs = new();
    }
}
