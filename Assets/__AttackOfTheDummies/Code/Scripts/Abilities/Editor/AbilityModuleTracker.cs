using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class AbilityModuleTracker
{
    private static readonly string ModuleGUIDMapPath = "Library/AbilityModuleGUIDMap.json";

    private static Dictionary<string, Type> guidToModuleType = new();
    private static Dictionary<Type, string> moduleTypeToGuid = new();

    static AbilityModuleTracker()
    {
        LoadGUIDMappings();
        EditorApplication.delayCall += DetectModuleChanges;
    }

    private static void LoadGUIDMappings()
    {
        if (File.Exists(ModuleGUIDMapPath))
        {
            string json = File.ReadAllText(ModuleGUIDMapPath);
            var mappings = JsonUtility.FromJson<ModuleGUIDMap>(json);

            guidToModuleType = mappings.entries
                .Select(e => new { e.guid, type = Type.GetType(e.typeName) })
                .Where(e => e.type != null)
                .ToDictionary(e => e.guid, e => e.type);

            moduleTypeToGuid = guidToModuleType.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }
    }

    private static void SaveGUIDMappings()
    {
        var mappings = new ModuleGUIDMap
        {
            entries = moduleTypeToGuid.Select(kvp => new ModuleGUIDEntry
            {
                guid = kvp.Value,
                typeName = kvp.Key.AssemblyQualifiedName
            }).ToList()
        };

        File.WriteAllText(ModuleGUIDMapPath, JsonUtility.ToJson(mappings, true));
    }

    public static object RestoreOrCreate(Type moduleType)
    {
        if (!moduleTypeToGuid.TryGetValue(moduleType, out string guid))
        {
            guid = GenerateStableGUID(moduleType);
            moduleTypeToGuid[moduleType] = guid;
            guidToModuleType[guid] = moduleType;
            SaveGUIDMappings();
        }

        return Activator.CreateInstance(moduleType);
    }

    private static string GenerateStableGUID(Type moduleType)
    {
        using (var md5 = MD5.Create())
        {
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(moduleType.AssemblyQualifiedName));
            return new Guid(hash).ToString();
        }
    }

    private static void DetectModuleChanges()
    {
        var knownTypes = new HashSet<Type>(moduleTypeToGuid.Keys);
        var currentTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAbilityModule).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            .ToList();

        foreach (var type in currentTypes)
        {
            if (!knownTypes.Contains(type))
            {
                RestoreOrCreate(type);
            }
        }

        SaveGUIDMappings();
    }

    [Serializable]
    private class ModuleGUIDMap
    {
        public List<ModuleGUIDEntry> entries = new();
    }

    [Serializable]
    private class ModuleGUIDEntry
    {
        public string guid;
        public string typeName;
    }
}
