using System;
using UnityEngine;

[Serializable]
public struct AffectMask
{
    [SerializeField] private int mask;

    public int Mask => mask;

    public bool Contains(int factionIndex)
    {
        return (mask & (1 << factionIndex)) != 0;
    }

    public void Add(int factionIndex)
    {
        mask |= (1 << factionIndex);
    }

    public void Remove(int factionIndex)
    {
        mask &= ~(1 << factionIndex);
    }

    public void SetMask(int newMask)
    {
        mask = newMask;
    }

    public FactionMask GetRelevantFactions(GameObject source)
    {
        if(source.TryGetComponent(out EnemyAI enemy))
        {
            return enemy.faction;
        }

        if(source.TryGetComponent(out PlayerController playerController))
        {
            return playerController.faction;
        }

        return 0;
    }

    public static implicit operator int(AffectMask f) => f.mask;
    public static implicit operator AffectMask(int value) => new AffectMask { mask = value };
}