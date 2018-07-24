using System.Collections;

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        if (!unit.detection.detectedSomeone)
            yield break;

        Unit[] search = pFlag.VisibleUnits;
        float[] dists = transform.position.GetDistances(search);
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = search[closestUnitIndex].curSlot;
        if (closestUnit!= null) { // no player units
            yield break;
        }

        GridItem nearbySlot = unit.curSlot;
        if (!GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            nearbySlot = AiHelper.ClosestToAttackEdgeOverMoveMask(unit.curSlot, closestUnit, unit.pathing.moveMask, unit.abilities.additionalAbilities2[0].standard.attackRangeMask);

            if (nearbySlot == null)
                yield break;

            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));
                
            unit.MoveAction(nearbySlot);
            while (unit.moving) {
                yield return null;
            }
        }
        // command 2
        if (GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(closestUnit));

            unit.AttackAction2(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.additionalAbilities2[0]);
        }

        while (unit.attacking) {
            yield return null;
        }
        // end unit turn
        yield return null;
    }
}