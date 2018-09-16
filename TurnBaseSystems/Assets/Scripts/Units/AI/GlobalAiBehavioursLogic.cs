using System;
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
        targetEnemy = AiHelper.ClosestUnit(unit.snapPos, Combat.Instance.GetVisibleUnits(0).ToArray());
        targetAlly = AiHelper.ClosestUnit(unit.snapPos, Combat.Instance.GetVisibleUnits(1, unit).ToArray());

        Vector3 targetMovePos = Vector3.zero;
        Vector3 attackPos = Vector3.zero;

        Vector3 groupPreferedPos = Vector3.zero;
        AttackData2 ability = null;
        ability = unit.abilities.additionalAbilities2[0];
        Debug.Log("[Enemy AI] "+unit+" "+ mode + ", prefered enemy "+targetEnemy + " prefered ally "+targetAlly);
        // melee
        if (mode == 0) {
            // approach at closest and attack
            if (AiHelper.IsNeighbour(unit.snapPos, targetEnemy.snapPos)) { // don't move when already near
                targetMovePos = unit.snapPos;
            } else {
                targetMovePos = AiHelper.ClosestSlotToTargetOverMask(unit.snapPos, targetEnemy.snapPos, unit.abilities.move2.move.range);
            }
            if (targetEnemy)
                attackPos = targetEnemy.snapPos;
        }
        // ranged
        if (mode == 1) {
            RangedBehaviour(unit, targetEnemy, ref targetMovePos, ref attackPos, 0);
        }

        // patrol
        if (mode == 2) {
            // move between random points in small area, or return to that area if outside
            targetMovePos = AiHelper.RandomPointOnMask(patrolStartPos, patrolRange, unit.abilities.move2.move.range);
        }
        // grouping
        if (mode == 3) {
            // try to join group, by going towards closest ally
            groupPreferedPos = AiHelper.ClosestSlotToTargetOverMask(unit.snapPos, targetAlly.snapPos, unit.abilities.move2.move.range);
            Vector3 groupInfluencedPos = unit.snapPos + (groupPreferedPos- unit.snapPos)*closestAllyPreference + (targetMovePos - unit.snapPos);
            targetMovePos = AiHelper.ClosestSlotToTargetOverMask(unit.snapPos, groupInfluencedPos, unit.abilities.move2.move.range);
        }
        // healer
        if (mode == 4) {
            // heal closest damaged unit if any, else attack.
            Unit unhealedAlly = AiHelper.ClosestUnit(unit.snapPos, AiHelper.OnlyUnhealed(Combat.Instance.GetVisibleUnits(1, unit).ToArray()));
            if (unhealedAlly) {
                targetAlly = unhealedAlly;
                targetMovePos = AiHelper.ClosestSlotToTargetOverMask(unit.snapPos, targetAlly.snapPos, unit.abilities.move2.move.range);
                attackPos = targetAlly.snapPos;
            } else {
                RangedBehaviour(unit, targetEnemy, ref targetMovePos, ref attackPos, 1);
                ability = unit.abilities.additionalAbilities2[1];// assuming attack for healers is on 1
            }
        }


        yield return unit.StartCoroutine(DebugGrid.BlinkColor(targetMovePos));
        yield return unit.StartCoroutine(MoveToPos(unit, targetMovePos, unit.abilities.move2));
        yield return unit.StartCoroutine(DebugGrid.BlinkColor(attackPos));
        yield return unit.StartCoroutine(AttackPos(unit, attackPos, ability));

        yield return null;
    }

    private void RangedBehaviour(Unit unit, Unit targetEnemy, ref Vector3 targetMovePos, ref Vector3 attackPos, int abilityId) {
        // approach at furthest and attack
        targetMovePos = AiHelper.MaxRangeOnMask(unit.snapPos, targetEnemy.snapPos, unit.abilities.move2.move.range, unit.abilities.additionalAbilities2[abilityId].standard.attackRangeMask);

        if (targetEnemy)
            attackPos = targetEnemy.snapPos;
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
