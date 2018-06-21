using System.Collections;
public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        float[] dists = transform.position.GetDistances(pFlag.units.ToArray());
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = pFlag.units[closestUnitIndex].curSlot;
        GridItem nearbySlot;
        //if (AiHelper.IsNeighbour(unit.curSlot, closestUnit))// don't move when already near
        //    nearbySlot = unit.curSlot;
        //else 
        nearbySlot = AiHelper.ClosestFreeSlotOnEdge(transform.position, closestUnit, unit.abilities.BasicAttack.attackMask);
        if (nearbySlot == null)
            yield break;

        unit.MoveAction(nearbySlot);
        while (unit.moving) {
            yield return null;
        }
        // command 2
        unit.AttackAction(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.BasicAttack);
        // end unit turn
        yield return null;
    }
}