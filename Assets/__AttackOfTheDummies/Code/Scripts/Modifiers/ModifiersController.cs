using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    //try: key should be ModSignature?
    public Dictionary<Modifier, List<Modifier>> activeModifiers = new();

    public void ApplyModifier(Modifier mod, GameObject source)
    {
        mod.SetAffected(gameObject);
        mod.SetSource(source);

        Modifier modBase = Instantiate(mod);
        ApplyModifier(modBase);
    }

    public void ApplyModifier(Modifier modBase)
    {
        activeModifiers.Add(modBase, new List<Modifier>());
        modBase.Instantiate();
    }
}
