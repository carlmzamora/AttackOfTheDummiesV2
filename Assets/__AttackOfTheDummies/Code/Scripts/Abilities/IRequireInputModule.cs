public interface IRequireInputModule : IAbilityModule
{
    void ShowWaitingForInputDisplay();
    void UpdateWaitForInput();
    void EndWaitForInput();
}
