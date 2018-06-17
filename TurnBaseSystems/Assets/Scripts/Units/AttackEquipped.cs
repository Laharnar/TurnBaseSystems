[System.Serializable]
public class AttackEquipped : Attack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (source.equippedWeapon!=null && attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(source.equippedWeapon.massDamage);
        }
    }
}