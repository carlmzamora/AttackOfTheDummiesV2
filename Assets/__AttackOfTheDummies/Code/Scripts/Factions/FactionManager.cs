using UnityEngine;

public class FactionManager : MonoBehaviour
{
    [SerializeField] private FactionDatabase factionDatabase;

    public static FactionManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public FactionData GetFactionData(int factionIndex)
    {
        return factionDatabase?.GetFactionData(factionIndex);
    }

    public bool CanAffect(IAbilitiesHolder caster, GameObject target, AffectRule affectRule)
    {
        if (target == null) return false;

        if(caster.gameObject == target) return true;

        IFactioned targetIFactioned = target.GetComponent<IFactioned>();
        if (targetIFactioned == null) return false; 

        Faction targetFaction = targetIFactioned.Faction;
        Faction casterFaction = caster.Faction;

        bool isAlly = casterFaction.IsAlly(targetFaction);
        bool isEnemy = casterFaction.IsEnemy(targetFaction);
        bool isNeutral = !isAlly && !isEnemy;

        int targetType = (isAlly ? (int)AffectRule.Allies : 0) |
                         (isEnemy ? (int)AffectRule.Enemies : 0) |
                         (isNeutral ? (int)AffectRule.Neutral : 0);

        return (affectRule & (AffectRule)targetType) != 0;
    }
}
