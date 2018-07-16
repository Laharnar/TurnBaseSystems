﻿using System;
using System.Collections;
using UnityEngine;

public class MelleLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;
        
        if (!unit.detection.detectedSomeone)
            yield break;

        float[] dists = transform.position.GetDistances(pFlag.units.ToArray());
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = SelectionManager.GetAsSlot(pFlag.units[closestUnitIndex].transform.position);
        GridItem nearbySlot;
        if (AiHelper.IsNeighbour(unit.curSlot, closestUnit))// don't move when already near
            nearbySlot = unit.curSlot;
        else {
            if (!unit.pathing.moveMask)
                nearbySlot = AiHelper.ClosestFreeSlotToSlot(transform.position, closestUnit);
            else
                nearbySlot = AiHelper.ClosestToTargetOverMask(unit.curSlot, closestUnit, unit.pathing.moveMask);
                //nearbySlot = AiHelper.ClosestFreeSlotOnEdge(transform.position, closestUnit, unit.pathing.moveMask);
        }
        if (nearbySlot == null)
            yield break;

        unit.MoveAction(nearbySlot);
        while (unit.moving) {
            yield return null;
        }
        // command 2
        if (GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities[0].attackMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));

            unit.AttackAction(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.additionalAbilities[0]);
        }
        while (unit.attacking) {
            yield return null;
        }
        // end unit turn
        yield return null;
    }
}
