[System.Serializable]
public class PickItem : EnvirounmentalAttack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        // doesn't do damage, tries to pickup item from slot
        if (attackedSlot.fillAsPickup) {
            source.EquipAction(attackedSlot.fillAsPickup);
            attackedSlot.DetachPickupFromSlot();
        }
    }
}