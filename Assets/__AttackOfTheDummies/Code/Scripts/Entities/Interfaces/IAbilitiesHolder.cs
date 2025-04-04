using System.ComponentModel;
using UnityEngine;

public interface IAbilitiesHolder : IFactioned, IProjectileSource
{
    public AbilitiesController AbilitiesController { get; }
    public GameObject gameObject { get; }
    public Transform transform { get; }
    public MonoBehaviour mono { get; }
    public float outgoingDamageMultiplier { get; set; }
    public float outgoingHealMultiplier { get; set; }
}