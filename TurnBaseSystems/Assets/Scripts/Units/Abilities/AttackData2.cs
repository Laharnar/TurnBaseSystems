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
    /// Runs over all effects and activates them.
    /// Effects contain responses to activator.
    /// </summary>
    /// <param name="info"></param>
    public void ActivateAbility(AbilityInfo info) {
        foreach (var item in GetAbilityEffects()) {
            //if(CombatEventMask.CanActivate(activator, item.activator)) {
            if (item.used)
                item.AtkBehaviourExecute(info);
            //}
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
