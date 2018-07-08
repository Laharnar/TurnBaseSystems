[System.Serializable]
public class ThrowEquipped : AttackBaseType {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (source.equippedWeapon) {
            // throw at enemy = damage, at ally = pass, at ground = de equip
            if (attackedSlot.filledBy && source.flag != attackedSlot.filledBy.flag) {
                attackedSlot.filledBy.GetDamaged(source.equippedWeapon.thrownDamage);
                source.equippedWeapon.transform.position = attackedSlot.filledBy.transform.position;
                source.DeEquip();
            } else if (attackedSlot.filledBy && source.flag == attackedSlot.filledBy.flag) {
                source.PassWeapon(source.equippedWeapon, attackedSlot.filledBy);
            }else if (!attackedSlot.filledBy) {
                source.equippedWeapon.transform.position = attackedSlot.filledBy.transform.position;
                source.DeEquip();
            }
        }
        source.equippedWeapon = null;
        
    }
}


