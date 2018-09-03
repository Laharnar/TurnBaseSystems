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
        return;
    }
    public static void DebugEvents(string msg) {
        Debug.Log("EVENT: "+msg);
    }

    /// <summary>
    /// Standard click event.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="hoveredSlot"></param>
    /// <param name="activeAbility"></param>
    public static void ClickAction(Unit unit, Vector3 hoveredSlot, AttackData2 activeAbility) {
        CombatEvents.DebugEvents("CombatAction");
        // v1
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onAttack = !AbilityInfo.CurActivator.never;
        AbilityInfo.Instance.executingUnit = unit;

        AbilityInfo.AttackedSlot = hoveredSlot;
        AbilityInfo.AttackStartedAt = unit.snapPos;
        AbilityInfo.ActiveAbility = activeAbility;
        //Combat.RegisterAbilityUse(unit, hoveredSlot, activeAbility);


        //AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onMove = !AbilityInfo.CurActivator.never;
        //Combat.RegisterAbilityUse(unit, hoveredSlot, activeAbility);

        //AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onDamaged = !AbilityInfo.CurActivator.never;
        Combat.RegisterAbilityUse(unit, hoveredSlot, activeAbility);

        // v2
        Debug.Log("Turn data isn't saved anymore.");
        //int action = selectedPlayerUnit.AttackAction2(hoveredSlot, activeAbility);
        //Combat.RegisterAbilityUse(unit, hoveredSlot, activeAbility);


        CombatEvents.OnUnitActivatesAbility(unit);
    }

    public static void ActivateAbilitiesByActivator() {
        return;
        for (int i = 0; i < Combat.Instance.units.Count; i++) {
            Combat.Instance.units[i].RunAllAbilities2(AbilityInfo.CurActivator);
        }
    }

    public static void OnTurnStart(FlagManager flag) {
        DebugEvents("OnTurnStart");
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onAnyTurnStart = !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onEnemyTurnStart = flag.id == 1 && !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onPlayerTurnStart = flag.id == 0 && !AbilityInfo.CurActivator.never;
        /*ActivateAbilitiesForCurCombatState();
        CombatEvents.ActivateAbilitiesByActivator();
        foreach (var unit in flag.info.units) {
            unit.RunAllAbilities2();
        }*/
        flag.NullifyUnits();
        foreach (var item in flag.info.units) {
            item.OnTurnStart();
        }
    }

    public static void OnTurnEnd(FlagManager flag) {
        DebugEvents("OnTurnEnd");
        // end
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onAnyTurnEnd = !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onEnemyTurnEnd = flag.id == 1 && !AbilityInfo.CurActivator.never;
        AbilityInfo.CurActivator.onPlayerTurnEnd = flag.id == 0 && !AbilityInfo.CurActivator.never;
        /*ActivateAbilitiesForCurCombatState();
        CombatEvents.ActivateAbilitiesByActivator();


        foreach (var unit in flag.info.units) {
            unit.RunAllAbilities2();
        }*/
        flag.NullifyUnits();
        foreach (var item in flag.info.units) {

            item.OnUnitTurnEnd();
        }

        BuffManager.ConsumeBuffs(flag);

        AbilityInfo.Instance.Reset();
       
    }
    
    public static void OnUnitDamaged() {
        

        /*AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onDamaged = !AbilityInfo.CurActivator.never;
        ActivateAbilitiesForCurCombatState();
        CombatEvents.ActivateAbilitiesIfStateMatches();
        */
    }

    public static void OnUnitActivatesAbility(Unit unit) {
        DebugEvents("OnUnitActivatesAbility");
        
        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
        Combat.Instance.UnitNullCheck();
    }

    public static void ReapplyAuras(Vector3 oldPos, Vector3 newPos, Unit unit) {

        DebugEvents("ReapplyAuras");

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