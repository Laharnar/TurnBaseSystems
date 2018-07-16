using System.Collections;

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        if (!unit.detection.detectedSomeone)
            yield break;

        float[] dists = transform.position.GetDistances(pFlag.units.ToArray());
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = pFlag.units[closestUnitIndex].curSlot;
        GridItem nearbySlot = unit.curSlot;
        if (!GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities[0].attackMask)) {
            nearbySlot = AiHelper.ClosestToAttackEdgeOverMoveMask(unit.curSlot, closestUnit, unit.pathing.moveMask, unit.abilities.additionalAbilities[0].attackMask);

            if (nearbySlot == null)
                yield break;

            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));
                
            unit.MoveAction(nearbySlot);
            while (unit.moving) {
                yield return null;
            }
        }
        // command 2
        if (GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities[0].attackMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(closestUnit));

            unit.AttackAction(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.additionalAbilities[0]);
        }

        while (unit.attacking) {
            yield return null;
        }
        // end unit turn
        yield return null;
    }
}