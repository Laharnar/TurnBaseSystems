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

    internal override void AtkBehaviourExecute() {
        Execute();
    }

    public void Execute() {
        if (giveCharges) {
            CI.sourceExecutingUnit.AddCharges(this, UnityEngine.Random.Range(chargesAmtMin, chargesAmtMax));
        }
        if (canBackstab) {
            Unit[] units= backstabRange.GetUnits(CI.attackStartedAt);
            // backstab 1 unit
            int c = maxBackstabCount;
            for (int i = 0; i < units.Length; i++) {
                if (units[i].flag.allianceId!= CI.sourceExecutingUnit.flag.allianceId) {
                    units[i].GetDamaged(backstabDmg);
                    if (c == 0)
                        break;
                }
            }
        }
    }
}

