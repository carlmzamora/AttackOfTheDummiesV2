using UnityEngine;

public interface IRequireInputModule : IAbilityModule
{
    void UpdateWaitForInputDisplay(Vector2 worldPos);
    void EndWaitForInput(Vector2 worldPos);
}