using UnityEngine;

public interface ITargetedCastModule : IAbilityModule
{
    void StartWaitForInput(Vector2 mouseWorldPos);
    void UpdateWaitForInputDisplay(Vector2 mouseWorldPos);
    void EndWaitForInput(Vector2 mouseWorldPos);
}