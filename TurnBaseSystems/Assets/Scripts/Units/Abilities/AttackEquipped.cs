[System.Serializable]
public class AttackWithEquipped : AttackBaseType {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        /*if (source.equippedWeapon!=null && attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(source.equippedWeapon.damage);
        }*/
    }
}