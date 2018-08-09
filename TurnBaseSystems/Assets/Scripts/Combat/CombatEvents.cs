using UnityEngine;
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
    public static void ActivateValidAbilities() {
        for (int i = 0; i < CombatManager.m.units.Count; i++) {
            CombatManager.m.units[i].RunAllAbilities(CI.curActivator);
        }
    }


    public static void OnTurnStart(int allianceId) {
        CI.curActivator.Reset();
        CI.curActivator.onAnyTurnStart = !CI.curActivator.never;
        CI.curActivator.onEnemyTurnStart = allianceId == 1 && !CI.curActivator.never;
        CI.curActivator.onPlayerTurnStart = allianceId == 0 && !CI.curActivator.never;
        ActivateValidAbilities();

        for (int i = 0; i < FlagManager.flags[allianceId].units.Count; i++) {
            FlagManager.flags[allianceId].units[i].OnTurnStart();
        }

        /*for (int i = 0; i < units.Count; i++) {

            for (int j = 0; j < units[i].abilities.additionalAbilities2.Count; j++) {
                EmpowerAlliesData aura = units[i].abilities.additionalAbilities2[j].aura;
                if (units[i].abilities.additionalAbilities2[j].aura.used) {
                    aura.AtkBehaviourExecute();
                    
                }
            }
        }*/

    }

    public static void OnTurnEnd(int j) {
        // end
        CI.curActivator.Reset();
        CI.curActivator.onAnyTurnEnd = !CI.curActivator.never;
        CI.curActivator.onEnemyTurnEnd = j == 1 && !CI.curActivator.never;
        CI.curActivator.onPlayerTurnEnd = j == 0 && !CI.curActivator.never;
        ActivateValidAbilities();

        FlagManager.flags[j].NullifyUnits();
        for (int i = 0; i < FlagManager.flags[j].units.Count; i++) {
            FlagManager.flags[j].units[i].OnTurnEnd();
        }

        // buffs
        int faction = j;
        CI.curActivator.Reset();
        CI.curActivator.onAnyBuffTick = !CI.curActivator.never;
        ActivateValidAbilities();
        Debug.Log("move buff unticking into execute");

        BuffManager.ConsumeBuffs(j);

    }

    public static void OnUnitActivatesAbility(Unit unit) {
        CI.curActivator.Reset();
        CI.curActivator.onDamaged = !CI.curActivator.never;
        ActivateValidAbilities();

        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
        CombatManager.m.UnitNullCheck();
    }

    public static void CombatAction(Unit selectedPlayerUnit, Vector3 hoveredSlot, AttackData2 activeAbility) {
        // v1
        CI.curActivator.Reset();
        CI.curActivator.onAttack = !CI.curActivator.never;
        CI.sourceExecutingUnit = selectedPlayerUnit;

        CI.attackedSlot = hoveredSlot;
        CI.attackStartedAt = selectedPlayerUnit.snapPos;
        CI.activeAbility = activeAbility;
        ActivateValidAbilities();

        CI.curActivator.Reset();
        CI.curActivator.onMove = !CI.curActivator.never;
        ActivateValidAbilities();

        CI.curActivator.Reset();
        CI.curActivator.onDamaged = !CI.curActivator.never;
        ActivateValidAbilities();

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
        foreach (var combatUnit in CombatManager.m.units) {
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