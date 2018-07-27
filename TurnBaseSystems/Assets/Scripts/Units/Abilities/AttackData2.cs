﻿using System;
using UnityEngine;


[System.Serializable]
public sealed class AttackData2 : StdAttackData {
    public string o_attackName;
    public string detailedDescription;
    public int actionCost = 1;
    public bool requiresUnit = true;
    public StandardAttackData standard;
    public AOEAttackData aoe;
    public BUFFAttackData buff;
    public EmpowerAlliesData aura;

    public GridMask AttackMask {
        get {
            if (standard.used) {
                return standard.attackRangeMask;
            }
            return null;
        }
    }

    public AttackDataType[] GetAttacks() {
        AttackDataType[] attacks = new AttackDataType[3];
        if (standard.priority > -1) {
            if (standard.used)
                attacks[standard.priority] = standard;
        }
        if (aoe.priority > -1) {
            if (aoe.used)
                attacks[aoe.priority] = aoe;
        }
        if (buff.priority > -1) {
            if (buff.used)
                attacks[buff.priority] = buff;
        }
        return attacks;
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

        Debug.Log("Using attack "+data.o_attackName +" unit: "+source+
            " "+data.standard.used+" "+data.aoe.used+" "+ data.buff.used);
        //AttackDataType[] attacks = data.GetAttacks();


        // standard
        if (data.standard.used) {
            Unit u = GridAccess.GetUnitAtPos(attackedSlot);
            if (u) {
                u.GetDamaged(data.standard.damage);
            }
            source.combatStatus = data.standard.setStatus;
        }
        // aoe
        if (data.aoe.used) {
            GridMask mask = GridMask.RotateMask(data.aoe.aoeMask, PlayerFlag.m.mouseDirection);
            Vector3[] vec = mask.GetPositions(attackedSlot);
            for (int i = 0; i < vec.Length; i++) {
                Unit u = GridAccess.GetUnitAtPos(vec[i]);
                if (u)
                    u.GetDamaged(data.aoe.damage);
            }
            source.combatStatus = data.aoe.setStatus;
        }
        // buff
        if (data.buff.used) {
            ActivateBuff(source, data.buff);

            BuffManager.Register(source, data.buff);
            source.combatStatus = data.buff.setStatus;
            Debug.Log(source.combatStatus);
        }
    }

    private static void ActivateBuff(Unit source, BUFFAttackData buff) {
        if (buff.buffType == BuffType.Shielded) {
            source.AddShield(buff, buff.armorAmt);
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
        return maxLen;
    }
}
