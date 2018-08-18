using System;
using UnityEngine;
[System.Serializable]
public sealed class AttackData2 : StdAttackData {

    public string o_attackName;
    public bool active = true;
    public string detailedDescription;
    public int actionCost = 1;
    public AttackRequirments requirements = AttackRequirments.Any;

    // activators activate the effect from combat manager
    public CombatEventMask[] activators;
    public AbilityEffectTarget[] effects;

    public AttackRangeData range;
    public StandardAttackData standard;
    public AOEAttackData aoe;
    public BUFFAttackData buff;
    public EmpowerAlliesData aura;
    public MoveAttackData move;
    public PassiveData passive;
    public PierceAtkData pierce;
    public SpawnAttackData spawn;
    internal int id;

    public AbilityEffect[] GetAbilityEffects() {
        return new AbilityEffect[] {
            range,
            standard,
            aoe,
            buff,
            aura,
            move,
            passive,
            pierce,
            spawn
        };
    }

    /// <summary>
    /// Executes attack data.
    /// Included all types of attacks(normal, aoe, buff...)
    /// Modifies Unit.combatStatus
    /// </summary>
    /// <param name="source"></param>
    /// <param name="attackedSlot"></param>
    /// <param name="data"></param>
    public static void UseAttack(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (source == null || data==null) {
            Debug.Log("Error. source:" + (source == null) + " slot:" + attackedSlot + " data:"+(data == null));
        }
        if (data.active == false) {
            Debug.Log("Error, activating disabled ability. source:" + (source == null) + " slot:" + attackedSlot + " data:" + (data == null));
            return;
        }

        Debug.Log("Using attack "+data.o_attackName +" unit: "+source+
            " "+data.standard.used+" "+data.aoe.used+" "+ data.buff.used);
        //AbilityEffect[] attacks = data.GetAttacks();

        AbilityInfo.ActiveAbility = data;
        AbilityInfo.SourceExecutingUnit = source;
        AbilityInfo.AttackedSlot = attackedSlot;
        AbilityInfo.AttackStartedAt = source.snapPos;
        // standard
        if (data.standard.used) {
            data.standard.Execute();
        }
        // aoe
        if (data.aoe.used) {
            data.aoe.Execute();
        }
        // buff
        if (data.buff.used) {
            if (GridAccess.GetUnitAtPos(attackedSlot)) {
                data.buff.Register(source, GridAccess.GetUnitAtPos(attackedSlot));
            }
        }
        if (data.move.used) {
            data.move.Execute();
        }
        if (data.pierce.used) {
            data.pierce.Execute();
        }
        if (data.passive.used) {
            AbilityInfo.AttackStartedAt = attackedSlot;
            data.passive.Execute();
        }
        if (data.spawn.used) {
            data.spawn.Execute();
        }
        
    }


    /// <summary>
    /// Activates triggers and bools in anim controller.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="triggers"></param>
    public static void RunAnimations(Unit unit, int[] triggers) {
        unit.abilities.abilityAnimations.Run(unit, triggers);
    }

    public static float AnimLength(Unit unit, AttackData2 attack) {
        float maxLen = 0;
        float f1 = attack.standard.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.standard.animSets) : 0,
            f2 = attack.aoe.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.aoe.animSets) : 0,
            f3 = attack.buff.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.buff.animSets) : 0;
        if (attack.standard.used) maxLen = maxLen < f1 ? f1 : maxLen;
        if (attack.aoe.used) maxLen = maxLen < f2 ? f2 : maxLen;
        if (attack.buff.used) maxLen = maxLen < f3 ? f3 : maxLen;
        // todo: for dash
        return maxLen;
    }
}
