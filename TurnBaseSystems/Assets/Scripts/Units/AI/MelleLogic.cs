﻿using System;
using System.Collections;
using UnityEngine;

public class MelleLogic : AiLogic {
    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = Combat.Instance.flags[0].controller as PlayerFlag;

        if (!unit.detection.detectedSomeone || UnitStates.GetVisibleUnits( Combat.Instance.GetUnits(0)).Length == 0)
            yield break;

        // set up data
        Vector3 selfPos = unit.snapPos;

        // search to choose target
        
        // find closest visible enemy
        Unit[] visibleUnits = UnitStates.GetVisibleUnits(Combat.Instance.GetUnits(0));
        float[] dists = transform.position.GetDistances(visibleUnits);
        int closestUnitIndex = dists.GetIndexOfMin();

        Vector3 closestEnemyPos = visibleUnits[closestUnitIndex].snapPos;
        Vector3 enemyPos = closestEnemyPos;

        // choose move pos
        Vector3 targetMovePos;
        if (AiHelper.IsNeighbour(unit.transform.position, enemyPos)) { // don't move when already near
            targetMovePos = selfPos;
        }
        else {
            targetMovePos = AiHelper.ClosestToTargetOverMask(unit.transform.position, enemyPos, unit.abilities.move2.move.range);
        }

        // move
        if (targetMovePos != selfPos)
        {
            Debug.Log("Moving to " + targetMovePos, unit);
            Combat.Instance.CombatAction(unit, targetMovePos, unit.abilities.move2);
            //unit.MoveAction(targetMovePos);
            while (unit.moving) {
                yield return null;
            }
            selfPos = GridManager.SnapPoint(unit.transform.position);
        }

        // attack
        if (GridLookup.IsPosInMask(selfPos, enemyPos, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(enemyPos));

            Combat.Instance.CombatAction(unit, enemyPos, unit.abilities.additionalAbilities2[0]);
        }
        while (unit.attacking) {
            yield return null;
        }

        //Combat.Instance.UnitNullCheck();

        // end unit turn
        yield return null;
    }
}
