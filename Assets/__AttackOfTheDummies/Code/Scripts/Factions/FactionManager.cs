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

    public bool CanAffect(GameObject caster, GameObject target, AffectRule affectRule)
    {
        if (target == null) return false;

        Faction casterFaction = caster.GetComponent<IFactioned>().Faction;
        Faction targetFaction = target.GetComponent<IFactioned>().Faction;

        int targetType = 0;
        if (casterFaction.IsAlly(targetFaction))
            targetType |= (int)AffectRule.Allies;

        if (casterFaction.IsEnemy(targetFaction))
            targetType |= (int)AffectRule.Enemies;

        if (!casterFaction.IsAlly(targetFaction) && !casterFaction.IsEnemy(targetFaction))
            targetType |= (int)AffectRule.Neutral;

        return (affectRule & (AffectRule)targetType) != 0;
    }
}
