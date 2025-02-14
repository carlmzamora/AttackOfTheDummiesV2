using System;
using UnityEngine;

[Serializable]
public struct FactionMask
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

    public static implicit operator int(FactionMask f) => f.mask;
    public static implicit operator FactionMask(int value) => new FactionMask { mask = value };
}