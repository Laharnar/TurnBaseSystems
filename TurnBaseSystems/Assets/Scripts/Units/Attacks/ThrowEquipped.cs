﻿[System.Serializable]
public class ThrowEquipped : Attack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(source.equippedWeapon.thrownDamage);
            source.equippedWeapon.transform.position = attackedSlot.filledBy.transform.position;
        }
        source.equippedWeapon = null;

    }
}


