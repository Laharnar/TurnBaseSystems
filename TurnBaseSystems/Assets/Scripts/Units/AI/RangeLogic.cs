using System.Collections;
public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        float[] dists = transform.position.GetDistances(pFlag.units.ToArray());
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = pFlag.units[closestUnitIndex].curSlot;
        GridItem nearbySlot = unit.curSlot;
        if (!GridManager.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.BasicMask)) {
            nearbySlot = AiHelper.ClosestToAttackEdgeOverMoveMask(unit.curSlot, closestUnit, unit.pathing.moveMask, unit.abilities.BasicAttack.attackMask);

            if (nearbySlot == null)
                yield break;

            unit.MoveAction(nearbySlot);
            while (unit.moving) {
                yield return null;
            }
        }
        // command 2
        if (GridManager.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.BasicMask))
            unit.AttackAction(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.BasicAttack);
        // end unit turn
        yield return null;
    }
}