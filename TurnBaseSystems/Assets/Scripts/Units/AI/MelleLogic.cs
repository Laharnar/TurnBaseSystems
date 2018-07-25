using System;
using System.Collections;
using UnityEngine;

public class MelleLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;
        
        if (!unit.detection.detectedSomeone)
            yield break;


        Unit[] search = pFlag.VisibleUnits;
        float[] dists = transform.position.GetDistances(search);
        int closestUnitIndex = dists.GetIndexOfMin();
        Vector3 closestUnit = GridManager.SnapPoint(search[closestUnitIndex].transform.position); //SelectionManager.GetAsSlot(search[closestUnitIndex].transform.position);
        Vector3 nearbySlot;
        if (AiHelper.IsNeighbour(unit.transform.position, closestUnit))// don't move when already near
            nearbySlot = unit.transform.position;
        else {
            if (!unit.pathing.moveMask)
                nearbySlot = AiHelper.ClosestFreeSlotToSlot(transform.position, closestUnit);
            else
                nearbySlot = AiHelper.ClosestToTargetOverMask(unit.transform.position, closestUnit, unit.pathing.moveMask);
                //nearbySlot = AiHelper.ClosestFreeSlotOnEdge(transform.position, closestUnit, unit.pathing.moveMask);
        }
        if (nearbySlot == null)
            yield break;

        unit.MoveAction(nearbySlot);
        while (unit.moving) {
            yield return null;
        }
        // command 2
        if (GridLookup.IsPosInMask(nearbySlot, closestUnit, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));

            unit.AttackAction2(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.additionalAbilities2[0]);
        }
        while (unit.attacking) {
            yield return null;
        }
        CombatManager.m.UnitNullCheck();

        // end unit turn
        yield return null;
    }
}
