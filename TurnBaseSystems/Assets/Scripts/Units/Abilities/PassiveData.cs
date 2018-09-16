[System.Serializable]
public class PassiveData : DamageBasedAttackData {
    public bool canHeal = false;
    public int healAmt = 1;

    public bool giveCharges = false;
    public int chargesAmtMin = 1;
    public int chargesAmtMax = 1;

    public bool canBackstab = false;
    public GridMask backstabRange;
    public int backstabDmg = 1;
    public int maxBackstabCount = -1;

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if ((info.executingUnit.flag.allianceId == 0 &&
            info.activator.onEnemyTurnEnd)
            ||
            info.executingUnit.flag.allianceId == 1 &&
            info.activator.onPlayerTurnEnd
            || info.activator.onAnyTurnEnd) {
            Execute(info);
            //info.executingUnit.AbilitySuccess();
        }
    }

    public void Execute(AbilityInfo info) {
        if (giveCharges) {
            info.executingUnit.AddCharges(this, UnityEngine.Random.Range(chargesAmtMin, chargesAmtMax));
        }
        if (canBackstab) {
            Unit[] units= backstabRange.GetUnits(info.attackStartedAt);
            // backstab 1 unit
            int c = maxBackstabCount;
            for (int i = 0; i < units.Length; i++) {
                if (units[i].flag.allianceId!= info.executingUnit.flag.allianceId) {
                    units[i].GetDamaged(backstabDmg);
                    info.executingUnit.AbilitySuccess();
                    if (c == 0)
                        break;
                }
            }
        }
    }
}

