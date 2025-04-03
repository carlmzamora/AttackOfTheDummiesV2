using System.ComponentModel;

public interface IAbilitiesHolder : IFactioned, IProjectileSource
{
    public AbilitiesController AbilitiesController { get; }
    public float outgoingDamageMultiplier { get; set; }
    public float outgoingHealMultiplier { get; set; }
}