using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionDatabase : ScriptableObject
{
    private const int MAX_FACTIONS = 32;

    [SerializeField] private List<FactionData> factions = new List<FactionData>();

    public List<FactionData> GetAllFactionData()
    {
        return factions.Where(data => data.factionName != "").ToList();
    }

    public FactionData GetFactionData(int factionIndex)
    {
        if (factionIndex >= 0 && factionIndex < factions.Count)
            return factions[factionIndex];

        return null;
    }

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
    public FactionMask allyFactions;
}

[Serializable]
public class Faction
{
    public int factionIndex = -1;

    public bool IsAlly(Faction other)
    {
        if (other == null) return false;

        FactionData myFaction = FactionManager.Instance.GetFactionData(factionIndex);
        return myFaction != null && myFaction.allyFactions.Contains(other.factionIndex);
    }

    public bool IsEnemy(Faction other)
    {
        if (other == null) return false;

        FactionData myFaction = FactionManager.Instance.GetFactionData(factionIndex);
        return myFaction != null && myFaction.enemyFactions.Contains(other.factionIndex);
    }
}

[System.Flags]
public enum AffectRule
{
    None = 0,
    Allies = 1 << 0,   // 0001
    Enemies = 1 << 1,  // 0010
    Neutral = 1 << 2,  // 0100
    Any = Allies | Enemies | Neutral // 0111 (All bits set)
}