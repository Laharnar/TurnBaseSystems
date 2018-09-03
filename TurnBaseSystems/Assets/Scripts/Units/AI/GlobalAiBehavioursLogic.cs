using System.Collections;
using UnityEngine;
public class GlobalAiBehavioursLogic : AiLogic {
    public int mode = 0;

    Vector3 patrolStartPos;
    public float patrolRange = 10f;

    [Range(0f, 1f)]
    public float closestAllyPreference = 0f;

    private void Start() {
        patrolStartPos = transform.position;
    }

    public override IEnumerator Execute(Unit unit) { 
        // targeting 
        Unit targetEnemy = null;
        Unit targetAlly = null;
        targetEnemy = AiHelper.ClosestUnit(unit.snapPos, Combat.Instance.GetUnits(0).ToArray());
        targetAlly = AiHelper.ClosestUnit(unit.snapPos, Combat.Instance.GetUnits(1).ToArray());

        Vector3 targetMovePos = Vector3.zero;
        Vector3 attackPos = Vector3.zero;

        Vector3 groupPreferedPos = Vector3.zero;
        // melee
        if (mode == 0) {
            // approach at closest and attack
            if (AiHelper.IsNeighbour(unit.snapPos, targetEnemy.snapPos)) { // don't move when already near
                targetMovePos = unit.snapPos;
            } else {
                targetMovePos = AiHelper.ClosestToTargetOverMask(unit.snapPos, targetEnemy.snapPos, unit.abilities.move2.move.range);
            }
            if (targetEnemy)
                attackPos = targetEnemy.snapPos;
        }
        // ranged
        if (mode == 1) {
            // approach at furthest and attack
            targetMovePos = AiHelper.MaxRangeOnMask(unit.snapPos, targetEnemy.snapPos, unit.abilities.move2.move.range, unit.abilities.additionalAbilities2[0].standard.attackRangeMask);
            
            if (targetEnemy)
                attackPos = targetEnemy.snapPos;
        }

        // patrol
        if (mode == 2) {
            // move between random points in small area, or return to that area if outside
            targetMovePos = AiHelper.RandomPointOnMask(patrolStartPos, patrolRange, unit.abilities.move2.move.range);
        }
        // grouping
        if (mode == 3) {
            // try to join group, by going towards closest ally
            groupPreferedPos = AiHelper.ClosestToTargetOverMask(unit.snapPos, targetAlly.snapPos, unit.abilities.move2.move.range);
            Vector3 groupInfluencedPos = unit.snapPos + (groupPreferedPos- unit.snapPos)*closestAllyPreference + (targetMovePos - unit.snapPos);
            targetMovePos = AiHelper.ClosestToTargetOverMask(unit.snapPos, groupInfluencedPos, unit.abilities.move2.move.range);
        }

        AttackData2 ability = null;
        ability = unit.abilities.additionalAbilities2[0];

        yield return unit.StartCoroutine(MoveToPos(unit, targetMovePos, unit.abilities.move2));
        yield return unit.StartCoroutine(AttackPos(unit, attackPos, ability));

        yield return null;
    }

    private IEnumerator MoveToPos(Unit unit, Vector3 targetMovePos, AttackData2 moveAbility) {
        Debug.Log("Moving to " + targetMovePos, unit);
        if (GridLookup.IsPosInMask(unit.snapPos, targetMovePos, moveAbility.move.range)) {
            CombatEvents.ClickAction(unit, targetMovePos, moveAbility);
        }
        yield return null;
        //unit.MoveAction(targetMovePos);
        while (unit.moving) {
            yield return null;
        }
    }

    private IEnumerator AttackPos(Unit unit, Vector3 enemyPos, AttackData2 attackData2) {
        if (GridLookup.IsPosInMask(unit.snapPos, enemyPos, attackData2.standard.attackRangeMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(enemyPos));
            CombatEvents.ClickAction(unit, enemyPos, attackData2);
        }
        yield return null;
        while (unit.attacking) {
            yield return null;
        }
    }
}
