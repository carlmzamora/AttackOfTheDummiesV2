using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    public string displayName;
    //public Sprite abilityIcon;
    public float cooldown;
    [HideInInspector] public bool noAdditionalInput = true;

    protected GameObject owner;
    public int level;

    public virtual void Activate() { }
    public virtual void ShowWaitingForInputDisplay() { }
    public virtual void UpdateWaitForInput() { }
    public virtual void EndWaitForInput() { }

    public virtual void Setup(GameObject owner, int level = -1)
    {
        this.owner = owner;
        this.level = level;
    }
}