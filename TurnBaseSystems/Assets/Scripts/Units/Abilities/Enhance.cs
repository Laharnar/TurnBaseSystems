[System.Serializable]
public class Enhance : AttackBaseType {
    public int numOfTurns = 5;
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        /*if (source.equippedWeapon)
            source.equippedWeapon.Enhance(numOfTurns);*/
    }
}
