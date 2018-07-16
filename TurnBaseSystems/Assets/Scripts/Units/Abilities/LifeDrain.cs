[System.Serializable]
public class LifeDrain : AoeMaskAttack {
    public int restoreAPPerUnitHit = 2;
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {

        GridItem[] attackArea;
        attackArea = GridAccess.LoadLocalAoeAttackLayer(source, aoeMask, PlayerFlag.m.mouseDirection, attackedSlot);

        int unitsHit = 0;
        for (int i = 0; i < attackArea.Length; i++) {
            if (attackArea[i].filledBy) {
                attackArea[i].filledBy.GetDamaged(damage);
                unitsHit++;
            }
        }
        source.RestoreAP(unitsHit * restoreAPPerUnitHit);
    }
}
