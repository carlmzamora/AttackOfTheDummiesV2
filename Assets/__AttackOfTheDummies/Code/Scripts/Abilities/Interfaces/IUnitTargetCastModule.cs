using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitTargetCastModule : ITargetedCastModule
{
    bool CanTarget(GameObject candidate);
    void CastOnTarget(GameObject target);

    void OnInvalidTarget();
}
