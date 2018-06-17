[System.Serializable]
public class AttackWithEquipped : Attack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (source.equippedWeapon!=null && attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(source.equippedWeapon.damage);
        }
    }
}