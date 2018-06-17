public class Weapon: Attack {
    public float accuracy = 1;
    public int damage = 1;
    internal int massDamage;

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (UnityEngine.Random.Range(0f, 1f) <= accuracy) {
            if (attackedSlot.filledBy) {
                attackedSlot.filledBy.GetDamaged(damage);
            }
        }
    }
}
