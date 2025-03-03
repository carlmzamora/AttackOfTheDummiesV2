using UnityEngine;

public interface ITargetedCastModule : IAbilityModule
{
    void UpdateWaitForInputDisplay(Vector2 worldPos);
    void EndWaitForInput(Vector2 worldPos);
}