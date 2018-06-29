[System.Serializable]
public class PassEquipped : Attack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackedSlot.filledBy && source.equippedWeapon) {
            source.PassWeapon(source.equippedWeapon, attackedSlot.filledBy);
        }
    }
}
