using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : ScriptableObject
{
    [Header("General")]
    public string displayName;
    //public Sprite abilityIcon;
    public float cooldown;
    public bool instantCast = true; //consider protected?

    protected GameObject owner;
    public int level;

    //Turn these into ActiveAbility?
    public virtual void Activate() { }
    public virtual void ShowWaitingForInputDisplay() { }
    public virtual void UpdateWaitForInput(Vector2 worldPos, bool mouse1Pressed) { }
    public virtual void EndWaitForInput() { }

    public virtual void Setup(GameObject owner, int level = -1)
    {
        this.owner = owner;
        this.level = level;
    }
}