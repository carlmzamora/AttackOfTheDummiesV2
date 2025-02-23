public interface IRequireInputAbility : IAbilityModule
{
    void ShowWaitingForInputDisplay();
    void UpdateWaitForInput();
    void EndWaitForInput();
}
