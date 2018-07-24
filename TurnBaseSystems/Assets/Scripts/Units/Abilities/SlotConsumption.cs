[System.Serializable]
public class SlotConsumption : EnvirounmentalAttack {

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackedSlot.fillAsStructure) {
            /*source.materials += attackedSlot.fillAsStructure.materialValue;
            attackedSlot.RemoveInteractions(attackedSlot.fillAsStructure.GetComponent<InteractiveEnvirounment>().interactions);
            attackedSlot.fillAsStructure.Destruct();*/
        }
    }
}
