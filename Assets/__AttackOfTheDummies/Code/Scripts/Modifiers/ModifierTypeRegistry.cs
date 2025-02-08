using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ModifierTypeRegistry
{
    private static List<Type> modifierTypes;

    public static List<Type> GetModifierTypes()
    {
        if(modifierTypes == null)
        {
            modifierTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Modifier)))
                .ToList();
        }

        return modifierTypes;
    }
}
