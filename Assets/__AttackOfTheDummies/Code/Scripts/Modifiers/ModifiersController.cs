using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    //try: key should be ModSignature?
    public Dictionary<Type, List<Modifier>> activeModifiers = new();

    public void ApplyModifier(Modifier mod, GameObject source, Ability abilityRoot)
    {
        if (!FactionManager.Instance.CanAffect(source, gameObject, mod.affectRule)) return;

        mod.affected = gameObject.GetComponent<MonoBehaviour>();
        mod.controller = gameObject.GetComponent<ModifiersController>();
        mod.source = source.GetComponent<MonoBehaviour>();
        mod.abilityRoot = abilityRoot;

        ApplyModifier(mod);
    }

    public void ApplyModifier(Modifier modBase)
    {
        List<Modifier> currentModifier = ValidateModifierExistence(modBase.GetType());
        int instancesCount = currentModifier.Count;

        if(instancesCount <= 0)
        {
            modBase.Instantiate(modBase.independentStackTimers);
            currentModifier.Add(modBase);
        }
        else
        {
            //if you collect further applications into one instance only
            if (currentModifier[0].allowOnlyOneInstance)
            {
                Modifier firstInstance = currentModifier[0];
                int firstInstanceStackCount = firstInstance.currentStacks;

                //if you can collect infinite stacks
                if (firstInstance.maxStacks == 0)
                {
                    firstInstance.AddStack(firstInstance.independentStackTimers);

                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
                else if (firstInstance.maxStacks == 1) //if you can't collect stacks
                {
                    //refresh only on further applicatons
                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
                else if (firstInstance.maxStacks > 1) //if you can collect stacks up to a limit
                {
                    if (firstInstanceStackCount < firstInstance.maxStacks) //if still below limit
                    {
                        firstInstance.AddStack(firstInstance.independentStackTimers);
                    }

                    if (firstInstance.refreshOnReapply)
                        firstInstance.RefreshDuration();
                }
            }
            else //if further applications create multiple instances, eg. infernal blade
            {
                //probably currentModifier.Add(modifier)
            }
        }
    }

    public void RemoveModifier(Modifier modifierToRemove)
    {
        Type type = modifierToRemove.GetType();
        activeModifiers[type].Remove(modifierToRemove); //remove modifier from list of its type

        if (activeModifiers[type].Count <= 0) //if list of its type no longer has any active instances
        {
            activeModifiers.Remove(type); //it is now completely inactive, so should be removed
        }
    }

    private List<Modifier> ValidateModifierExistence(Type type)
    {
        if (!activeModifiers.ContainsKey(type))
        {
            List<Modifier> newModifierList = new();
            activeModifiers.Add(type, newModifierList);
            return newModifierList;
        }
        else
            return activeModifiers[type];
    }
}