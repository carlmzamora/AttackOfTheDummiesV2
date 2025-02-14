using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionDatabase : ScriptableObject
{
    private const int MAX_FACTIONS = 32;

    [SerializeField] private List<FactionData> factions = new List<FactionData>();

    public List<FactionData> GetFactions() => factions.Where(data => data.factionName != "").ToList();

    private void OnValidate()
    {
        if (factions.Count > MAX_FACTIONS)
        {
            factions.RemoveRange(MAX_FACTIONS, factions.Count - MAX_FACTIONS);
            Debug.LogError("FactionDatabase exceeded 32 factions. Extra factions were removed.");
        }
    }
}

[Serializable]
public class FactionData
{
    public string factionName;
    public FactionMask enemyFactions;
}