using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesController : MonoBehaviour
{
    public Ability slot1;

    private Ability slot1Instance;

    public void Start()
    {
        slot1Instance = Instantiate(slot1);
        slot1Instance.Setup(gameObject);
    }

    public void PerformMouse1()
    {
        slot1Instance.Activate();
    }
}