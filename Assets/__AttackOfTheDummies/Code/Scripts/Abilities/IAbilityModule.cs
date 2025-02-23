using UnityEngine;

public interface IAbilityModule
{
    ActiveAbility rootAbility { get; set; }
    void Setup(GameObject owner, int level = -1);
}