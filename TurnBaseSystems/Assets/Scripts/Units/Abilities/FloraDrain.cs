[System.Serializable]
public class FloraDrain : AoeMaskAttack {
    public int restoreAPPerSlotHit = 2;
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {

        GridItem[] attackArea;
        attackArea = GridAccess.LoadLocalAoeAttackLayer(source, aoeMask, PlayerFlag.m.mouseDirection, attackedSlot);

        int groundHits = 0;
        for (int i = 0; i < attackArea.Length; i++) {
            if (!attackArea[i].filledBy) {
                if(attackArea[i].TryDrainGround())
                    groundHits++;
            }
        }
        source.RestoreAP(groundHits * restoreAPPerSlotHit);
    }
}
