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
    static void ActivateAbilitiesForCurCombatState() {
        for (int i = 0; i < Combat.Instance.units.Count; i++) {
            Combat.Instance.units[i].RunAllAbilities(CI.curActivator);
        }
    }


    public static void OnTurnStart(FlagManager flag) {
        CI.curActivator.Reset();
        CI.curActivator.onAnyTurnStart = !CI.curActivator.never;
        CI.curActivator.onEnemyTurnStart = flag.id == 1 && !CI.curActivator.never;
        CI.curActivator.onPlayerTurnStart = flag.id == 0 && !CI.curActivator.never;
        ActivateAbilitiesForCurCombatState();

        flag.NullifyUnits();
        foreach (var item in flag.info.units) {
            item.OnTurnStart();
        }
    }

    public static void OnTurnEnd(FlagManager flag) {
        // end
        CI.curActivator.Reset();
        CI.curActivator.onAnyTurnEnd = !CI.curActivator.never;
        CI.curActivator.onEnemyTurnEnd = flag.id == 1 && !CI.curActivator.never;
        CI.curActivator.onPlayerTurnEnd = flag.id == 0 && !CI.curActivator.never;
        ActivateAbilitiesForCurCombatState();

        flag.NullifyUnits();
        foreach (var item in flag.info.units) {
            item.OnTurnEnd();
        }

        //BuffManager.ConsumeBuffs(flag);

    }

    public static void OnUnitActivatesAbility(Unit unit) {
        CI.curActivator.Reset();
        CI.curActivator.onDamaged = !CI.curActivator.never;
        ActivateAbilitiesForCurCombatState();

        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
        Combat.Instance.UnitNullCheck();
    }

    public static void CombatAction(Unit selectedPlayerUnit, Vector3 hoveredSlot, AttackData2 activeAbility) {
        // v1
        CI.curActivator.Reset();
        CI.curActivator.onAttack = !CI.curActivator.never;
        CI.sourceExecutingUnit = selectedPlayerUnit;

        CI.attackedSlot = hoveredSlot;
        CI.attackStartedAt = selectedPlayerUnit.snapPos;
        CI.activeAbility = activeAbility;
        ActivateAbilitiesForCurCombatState();

        CI.curActivator.Reset();
        CI.curActivator.onMove = !CI.curActivator.never;
        ActivateAbilitiesForCurCombatState();

        CI.curActivator.Reset();
        CI.curActivator.onDamaged = !CI.curActivator.never;
        ActivateAbilitiesForCurCombatState();

        // v2
        int action = selectedPlayerUnit.AttackAction2(hoveredSlot, activeAbility);

        if (activeAbility == selectedPlayerUnit.abilities.move2) {// move
            OnUnitExecutesMoveAction(selectedPlayerUnit.snapPos, hoveredSlot, selectedPlayerUnit);
        }
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