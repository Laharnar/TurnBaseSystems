using System;
[System.Serializable]
public class AoeMaskAttack : AoeAttack{
    public int damage = 1;

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
       
        /*GridItem[] attackArea;
        attackArea = GridAccess.LoadLocalAoeAttackLayer(attackedSlot, aoeMask, PlayerFlag.m.mouseDirection);

        for (int i = 0; i < attackArea.Length; i++) {
            if (attackArea[i].filledBy)
                attackArea[i].filledBy.GetDamaged(damage);
        }*/
    }
}
