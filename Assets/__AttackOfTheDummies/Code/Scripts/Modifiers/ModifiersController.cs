using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifiersController : MonoBehaviour
{
    public Dictionary<Guid, Modifier> activeModifiers = new();
    public Dictionary<Type, List<Guid>> modifiersByType = new();

    public void ApplyModifier(Modifier mod, IAbilitiesHolder source, Ability abilityRoot)
    {
        mod.affected = GetComponent<MonoBehaviour>();
        mod.controller = this;
        mod.source = source.mono;
        mod.abilityRoot = abilityRoot;

        Type type = mod.GetType();
        Guid id = mod.id;

        //get modifier list or create if non-existent
        if (!modifiersByType.TryGetValue(type, out var modifierOfType))
        {
            modifierOfType = new List<Guid>();
            modifiersByType[type] = modifierOfType;
        }

        //first application
        if(!mod.allowOnlyOneInstance || modifierOfType.Count <= 0)
        {
            //register modifier to activeList and typeList
            activeModifiers.Add(id, mod);
            modifierOfType.Add(id);

            mod.Instantiate();
        }        
        else if (mod.allowOnlyOneInstance && modifierOfType.Count > 0) //on subsequent applications
        {
            Modifier firstInstance = activeModifiers[modifierOfType[0]];
            HandleStackingAndRefresh(firstInstance);
        }
    }

    private void HandleStackingAndRefresh(Modifier firstInstance)
    {
        if (firstInstance.maxStacks == 0)
        {
            firstInstance.AddStack();
        }
        else if (firstInstance.maxStacks == 1)
        {
            //do nothing
        }
        else if (firstInstance.maxStacks > 1)
        {
            if (firstInstance.currentStacks < firstInstance.maxStacks)
                firstInstance.AddStack();
        }

        if (firstInstance.refreshOnReapply)
            firstInstance.RefreshDuration();
    }

    public void UnregisterModifierFromActiveList(Modifier mod)
    {
        Guid id = mod.id;
        Type type = mod.GetType();

        activeModifiers.Remove(id);

        if (modifiersByType.TryGetValue(type, out var list))
        {
            list.Remove(id);

            if (list.Count == 0)
            {
                modifiersByType.Remove(type);
            }
        }
    }

    public void RemoveAllModifiers()
    {
        var modifierIDs = activeModifiers.Keys.ToList();

        foreach (var id in modifierIDs)
        {
            if (activeModifiers.TryGetValue(id, out var mod))
            {
                mod.CancelAndExpire();
            }
        }

        activeModifiers.Clear();
        modifiersByType.Clear();
    }
}