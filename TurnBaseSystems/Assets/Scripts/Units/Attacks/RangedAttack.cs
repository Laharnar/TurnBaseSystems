using UnityEngine;

[System.Serializable]
public class RangedAttack : Attack {

    public int damage = 1;

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackedSlot.filledBy) {
            attackedSlot.filledBy.GetDamaged(damage);
        } else {
            Debug.Log("Not unit in slot. Change tag to Normal");
        }
    }

}
