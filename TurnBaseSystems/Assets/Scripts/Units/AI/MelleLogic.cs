using System;
using System.Collections;
using UnityEngine;

public class MelleLogic : AiLogic {
    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        if (!unit.detection.detectedSomeone || pFlag.VisibleUnits.Length == 0)
            yield break;

        // find closest visible enemy
        Unit[] visibleUnits = pFlag.VisibleUnits;
        
        float[] dists = transform.position.GetDistances(visibleUnits);
        int closestUnitIndex = dists.GetIndexOfMin();
        Vector3 closestEnemyPos = GridManager.SnapPoint(visibleUnits[closestUnitIndex].transform.position); //SelectionManager.GetAsSlot(search[closestUnitIndex].transform.position);

        // set up data
        Vector3 selfPos = GridManager.SnapPoint(unit.transform.position);
        Vector3 enemyPos = closestEnemyPos;

        Vector3 targetMovePos;

        if (AiHelper.IsNeighbour(unit.transform.position, enemyPos)) { // don't move when already near
            targetMovePos = selfPos;
        }
        else {
            targetMovePos = AiHelper.ClosestToTargetOverMask(unit.transform.position, enemyPos, unit.abilities.move2.standard.attackRangeMask);
        }
        if (targetMovePos != selfPos)
        {
            Debug.Log("Moving to " + targetMovePos, unit);
            CombatManager.CombatAction(unit, targetMovePos, unit.abilities.move2);
            //unit.MoveAction(targetMovePos);
            while (unit.moving) {
                yield return null;
            }
            selfPos = GridManager.SnapPoint(unit.transform.position);
        }
        // command 2
        if (GridLookup.IsPosInMask(selfPos, enemyPos, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(enemyPos));

            CombatManager.CombatAction(unit, enemyPos, unit.abilities.additionalAbilities2[0]);
        }
        while (unit.attacking) {
            yield return null;
        }
        CombatManager.m.UnitNullCheck();

        // end unit turn
        yield return null;
    }
}
