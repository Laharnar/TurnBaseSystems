[System.Serializable]
public class PickItem : EnvirounmentalAttack {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        // doesn't do damage, tries to pickup item from slot
        if (attackedSlot.fillAsPickup) {
            GridManager.RecolorMask(source.curSlot, 0, source.abilities.BasicAttack.attackMask);
            source.EquipAction(attackedSlot.fillAsPickup);
            attackedSlot.DetachPickupFromSlot();
        }
    }
}