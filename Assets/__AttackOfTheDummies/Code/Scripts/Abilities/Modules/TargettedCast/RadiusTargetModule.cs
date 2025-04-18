using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiusTargetModule : AbilityModule, ITargetedCastModule
{
    public float radius;
    public bool hasGlobalCastRange;
    public float castRange;

    [SerializeField] public RadiusReticle customRadiusReticle;

    private RadiusReticle currentReticle;

    [Space(10)]
    [SerializeReference, HideAffectRule] public List<IApplicationEffect> effectsToApplyOnSelfOnCast = new();
    [Space(10)]
    [SerializeReference] public List<IApplicationEffect> effectsToApplyToTargetsInRadiusOnCast = new();

    public void StartWaitForInput(Vector3 mouseWorldPos)
    {
        currentReticle = TargetingVisualizer.Instance.RadiusReticle;

        currentReticle.SetPosition(mouseWorldPos);
        currentReticle.SetRadius(radius);
        currentReticle.gameObject.SetActive(true);
    }

    public void UpdateWaitForInput(Vector3 mouseWorldPos, bool confirmButtonPressed)
    {
        currentReticle.SetPosition(mouseWorldPos);
    }

    public void ConcludeWaitForInput(Vector3 mouseWorldPos)
    {
        foreach (IApplicationEffect effect in effectsToApplyOnSelfOnCast)
        {
            effect.ApplyEffect(owner.gameObject, owner, rootAbility);
        }

        Collider[] collidersInRadius = Physics.OverlapSphere(mouseWorldPos, radius, ~LayerMask.GetMask("Environment", "Ground"));

        foreach(Collider collider in collidersInRadius)
            Debug.Log(collider.gameObject.name);

        foreach (IApplicationEffect effect in effectsToApplyToTargetsInRadiusOnCast)
        {
            for (int i = 0; i < collidersInRadius.Length; i++)
            {
                effect.ApplyEffect(collidersInRadius[i].gameObject, owner, rootAbility);
            }
        }

        currentReticle.gameObject.SetActive(false);
    }

    public void CancelWaitForInput()
    {
        currentReticle.gameObject.SetActive(false);
    }
}
