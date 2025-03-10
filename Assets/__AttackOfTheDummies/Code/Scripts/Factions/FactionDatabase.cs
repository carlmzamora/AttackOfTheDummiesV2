using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionDatabase : ScriptableObject
{
    private const int MAX_FACTIONS = 32;

    [SerializeField] private List<Faction> factions = new List<Faction>();
    [HideInInspector] public List<Faction> GetFactions() => factions.Where(data => data.factionName != "").ToList();

    [SerializeField] private string[] affectTypes = { "ALLIES", "ENEMIES" };
    public string[] GetAffectTypes() => affectTypes;

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
public class Faction
{
    public string factionName;
    public FactionMask enemyFactions;
    public FactionMask allyFactions;
}