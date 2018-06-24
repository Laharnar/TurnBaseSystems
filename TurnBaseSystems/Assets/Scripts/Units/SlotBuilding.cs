[System.Serializable]
public class SlotBuilding : EnvirounmentalAttack {

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (!attackedSlot.fillAsStructure) {
            source.materials = 0;
            attackedSlot.CreateWall();
        }
    }
}
