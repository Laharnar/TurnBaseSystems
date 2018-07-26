using System.Collections;
using UnityEngine;

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;

        if (!unit.detection.detectedSomeone || pFlag.VisibleUnits.Length == 0)
            yield break;

        Unit[] search = pFlag.VisibleUnits;
        float[] dists = transform.position.GetDistances(search);
        int closestUnitIndex = dists.GetIndexOfMin();
        Unit closestUnit = search[closestUnitIndex];
        if (closestUnit!= null) { // no player units
            yield break;
        }

        Vector3 selfPos = GridManager.SnapPoint(unit.transform.position);
        Vector3 enemyPos = GridManager.SnapPoint(closestUnit.transform.position);
        Vector3 targetMovePos;
        if (!GridLookup.IsPosInMask(selfPos, enemyPos, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            targetMovePos = AiHelper.MaxRangeOnMask(selfPos, closestUnit.transform.position, unit.pathing.moveMask, unit.abilities.additionalAbilities2[0].standard.attackRangeMask);

            yield return unit.StartCoroutine(DebugGrid.BlinkColor(targetMovePos));
                
            unit.MoveAction(targetMovePos);
            while (unit.moving) {
                yield return null;
            }
             selfPos = GridManager.SnapPoint(unit.transform.position);
        }
        // command 2
        if (GridLookup.IsPosInMask(selfPos, enemyPos, unit.abilities.additionalAbilities2[0].standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(enemyPos));

            unit.AttackAction2(enemyPos, unit.abilities.additionalAbilities2[0]);
        }

        while (unit.attacking) {
            yield return null;
        }
        CombatManager.m.UnitNullCheck();

        // end unit turn
        yield return null;
    }
}