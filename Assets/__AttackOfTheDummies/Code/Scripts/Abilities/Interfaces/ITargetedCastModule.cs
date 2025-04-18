using UnityEngine;

public interface ITargetedCastModule : IAbilityModule
{
    void StartWaitForInput(Vector3 mouseWorldPos);
    void UpdateWaitForInput(Vector3 mouseWorldPos, bool confirmButtonPressed);
    void ConcludeWaitForInput(Vector3 mouseWorldPos);
    void CancelWaitForInput();
}