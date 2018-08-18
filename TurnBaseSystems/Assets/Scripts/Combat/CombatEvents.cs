using UnityEngine;

/// <summary>
/// Contains logic for all events that can happen in the game
/// </summary>
public static class CombatEvents {
    /*never,
        onAnyBuffTick,
        onAllyBuffTick,
        onHostileBuffTick,
        onPlayerTurnStart,
        onPlayerTurnEnd,
        onDamaged,
        onMove,
        onAttack,
        onStepOnEnemy,
        onEnemyEnterRange,
        onAllyEnterRange,
        onEnemyExitRange,
        onAllyExitRange,
        onCooldownTick, 
        onUnitDies
     * 
     * */
     [System.Obsolete("too hard to edit by hand")]
    public static void ActivateAbilitiesForCurCombatState() {
        for (int i = 0; i < Combat.Instance.units.Count; i++) {
            Combat.Instance.units[i].RunAllAbilities(AbilityInfo.CurActivator);
        }
    }


    public static void OnTurnStart(FlagManager flag) {
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onAnyTurnStart = !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onEnemyTurnStart = flag.id == 1 && !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onPlayerTurnStart = flag.id == 0 && !AbilityInfo.CurActivator.never;
        ActivateAbilitiesForCurCombatState();

        flag.NullifyUnits();
        foreach (var item in flag.info.units) {
            item.OnTurnStart();
        }
    }

    public static void OnTurnEnd(FlagManager flag) {
        // end
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onAnyTurnEnd = !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onEnemyTurnEnd = flag.id == 1 && !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onPlayerTurnEnd = flag.id == 0 && !AbilityInfo.CurActivator.never;
        ActivateAbilitiesForCurCombatState();

        flag.NullifyUnits();
        foreach (var item in flag.info.units) {
            item.OnTurnEnd();
        }

        BuffManager.ConsumeBuffs(flag);
    }

    public static void OnUnitActivatesAbility(Unit unit) {
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onDamaged = !AbilityInfo.CurActivator.never;
        ActivateAbilitiesForCurCombatState();

        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
        Combat.Instance.UnitNullCheck();
    }

    public static void OnUnitExecutesMoveAction(Vector3 oldPos, Vector3 newPos, Unit unit) {


        // on enter - on exit.
        // deapply and reapply auras from this all unit to all others
        EmpowerAlliesData.DeffectEffect(oldPos, newPos, unit, AuraTrigger.OnUnitEntersExits);

        // find auras that affected this unit at old pos, and add at new pos
        foreach (var combatUnit in Combat.Instance.units) {
            Vector3 snap = GridManager.SnapPoint(combatUnit.transform.position);
            if (combatUnit != unit) {
                foreach (var ability in combatUnit.abilities.additionalAbilities2) {
                    if (ability.aura.used && ability.aura.trigger == AuraTrigger.OnUnitEntersExits) {
                        bool inOld = ability.aura.auraRange.IsPosInMask(snap, oldPos);
                        bool inNew = ability.aura.auraRange.IsPosInMask(snap, newPos);
                        if (inOld && !inNew) {
                            ability.aura.LoseEffect(unit, combatUnit);
                        }
                        if (!inOld && inNew) {
                            ability.aura.Effect(unit, combatUnit);
                        }
                    }
                }
            }
        }
    }
}