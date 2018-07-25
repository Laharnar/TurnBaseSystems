using System.Collections;
using UnityEngine;

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        if (!unit.detection.detectedSomeone)
            yield break;

        Unit[] search = pFlag.VisibleUnits;
        float[] dists = transform.position.GetDistances(search);
        int closestUnitIndex = dists.GetIndexOfMin();
        Unit closestUnit = search[closestUnitIndex];
        if (closestUnit!= null) { // no player units
            yield break;
        }

        Vector3 nearbySlot = GridManager.SnapPoint(unit.transform.position);
        if (!GridLookup.IsPosInMask(nearbySlot, closestUnit.transform.position, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            nearbySlot = AiHelper.ClosestToAttackEdgeOverMoveMask(nearbySlot, closestUnit.transform.position, unit.pathing.moveMask, unit.abilities.additionalAbilities2[0].standard.attackRangeMask);

            if (nearbySlot == null)
                yield break;

            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));
                
            unit.MoveAction(nearbySlot);
            while (unit.moving) {
                yield return null;
            }
        }
        // command 2
        if (GridLookup.IsPosInMask(nearbySlot, closestUnit.transform.position, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));

            unit.AttackAction2(nearbySlot, pFlag.units[closestUnitIndex], unit.abilities.additionalAbilities2[0]);
        }

        while (unit.attacking) {
            yield return null;
        }
        // end unit turn
        yield return null;
    }
}