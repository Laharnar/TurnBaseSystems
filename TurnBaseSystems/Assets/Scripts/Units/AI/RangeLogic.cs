using System.Collections;
using UnityEngine;

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = Combat.Instance.flags[0].controller as PlayerFlag;

        if (!unit.detection.detectedSomeone || UnitStates.GetVisibleUnits(Combat.Instance.flags[0].info.units).Length == 0) {
            Debug.Log("Nothing detected yet, or no enemies on " +unit.name);
            yield break;
        }

        Unit[] visibleUnits = UnitStates.GetVisibleUnits(Combat.Instance.flags[0].info.units);
        float[] dists = transform.position.GetDistances(visibleUnits);
        int closestUnitIndex = dists.GetIndexOfMin();

        Unit closestUnit = visibleUnits[closestUnitIndex];
        if (closestUnit== null) { // no player units
            Debug.Log("no visible units", unit);
            yield break;
        }

        Vector3 selfPos = GridManager.SnapPoint(unit.transform.position);
        Vector3 enemyPos = GridManager.SnapPoint(closestUnit.transform.position);
        Vector3 targetMovePos;

        targetMovePos = AiHelper.MaxRangeOnMask(selfPos, closestUnit.transform.position, unit.abilities.move2.move.range, unit.abilities.additionalAbilities2[0].standard.attackRangeMask);
        Debug.Log(targetMovePos);

        yield return unit.StartCoroutine(DebugGrid.BlinkColor(targetMovePos));

        CombatEvents.ClickAction(unit, targetMovePos, unit.abilities.move2);
        //unit.MoveAction(targetMovePos);
        while (unit.moving) {
            yield return null;
        }
        selfPos = GridManager.SnapPoint(unit.transform.position);
        
        // command 2
        if (GridLookup.IsPosInMask(selfPos, enemyPos, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(enemyPos));

            CombatEvents.ClickAction(unit, enemyPos, unit.abilities.additionalAbilities2[0]);
        }

        while (unit.attacking) {
            yield return null;
        }
        Combat.Instance.UnitNullCheck();

        // end unit turn
        yield return null;
    }
}