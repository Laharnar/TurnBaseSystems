[System.Serializable]
public class RestoreAP : PassiveAttack {
    public int restoreAP = 1;
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        source.RestoreAP(restoreAP);
    }
}
