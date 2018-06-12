using System;
using System.Collections;
using UnityEngine;

public class MelleLogic : AiLogic {
    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        float[] dists = transform.position.GetDistances(pFlag.units);
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnitSlot = SelectionManager.GetAsSlot(pFlag.units[closestUnitIndex].transform.position);
        GridItem nearbySlot = AiHelper.ClosestFreeSlotToSlot(transform.position, closestUnitSlot);
        if (nearbySlot == null)
            yield break;

        unit.MoveAction(nearbySlot);
        while (unit.moving) {
            yield return null;
        }
        // command 2
        unit.AttackAction(closestUnitSlot, pFlag.units[closestUnitIndex], unit.abilities.BasicAttack);
        // end unit turn
        yield return null;
    }
}
