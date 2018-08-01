[System.Serializable]
public class PassiveData : AttackDataType {
    public bool canHeal = false;
    public int healAmt = 1;

    public bool canBackstab = false;
    public GridMask backstabRange;
    public int backstabDmg = 1;

    public void Execute(CurrentActionData a, AttackData2 ability) {
        if (canBackstab) {
            Unit[] units= ability.passive.backstabRange.GetUnits(a.attackStartedAt);
            // backstab 1 unit
            for (int i = 0; i < units.Length; i++) {
                if (units[i].flag.allianceId!=a.sourceExecutingUnit.flag.allianceId) {
                    units[i].GetDamaged(backstabDmg);
                    break;
                }
            }
        }
    }
}

