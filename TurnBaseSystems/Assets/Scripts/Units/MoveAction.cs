public class MoveAction : AttackBaseType {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (source.CanMoveTo(attackedSlot))
            source.MoveAction(attackedSlot);
    }
}

