[System.Serializable]
public class AOEAttack : Attack {

    public int damage = 1;
    public float range = 3f; // max 3 slots in 1 direction
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (!requiresUnit || attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(damage);
        }
    }
}