[System.Serializable]
public class PickItem : Attack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        // doesn't do damage, tries to pickup item from slot
        if (attackedSlot.fillAsPickup) {
            source.Equip(attackedSlot.fillAsPickup);
            attackedSlot.DetachPickupFromSlot();
        }
    }
}