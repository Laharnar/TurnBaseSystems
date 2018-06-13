[System.Serializable]
public class RangedAttack : Attack {

    public int damage = 1;

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackedSlot.filledBy ) {
            attackedSlot.filledBy.GetDamaged(damage);
        }
    }

}
