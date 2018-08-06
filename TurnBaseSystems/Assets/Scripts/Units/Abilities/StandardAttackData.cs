﻿using System;
using UnityEngine;
[System.Serializable]
public class StandardAttackData : DamageBasedAttackData {
    public int damage=1;
    public int heal = 0;
    public DamageInfo dmgInfo = new DamageInfo(1);

    public bool usePercentDmg = false;
    /// <summary>
    /// Dmg to cur hp. 100% will 1 shot. 99% = 1 hp
    /// </summary>
    public float percentDmg = 0f;
    [System.Obsolete]
    public GridMask attackRangeMask;
    public int healOnKills = 0;
    public static float dmgReduction = 0;
    public AuraTarget targets = AuraTarget.All;

    public GridMask GetMask(int direction) {
        return GridMask.RotateMask(attackRangeMask, direction);
    }
    internal void Execute(CurrentActionData actionData) {
        curDmg = dmgInfo;
        dmgReduction = Mathf.Clamp(dmgReduction, 0f, 1f);
        AttackData2 data = CombatInfo.activeAbility;
        Unit u = GridAccess.GetUnitAtPos(actionData.attackedSlot);
        if (u && EmpowerAlliesData.ValidTarget(targets, u.flag.allianceId, CombatInfo.attackingUnit )) {
            if (data.standard.usePercentDmg) {
                if (data.standard.percentDmg > 0f)
                    u.GetDamaged(Mathf.FloorToInt(((float)u.hp + (float)u.temporaryArmor) * data.standard.percentDmg * (1f - dmgReduction)));
            } else if (data.standard.damage > 0)
                u.GetDamaged((int)((float)data.standard.damage * (1f - dmgReduction)));
            if (heal > 0) {
                u.Heal(heal, this);
            }
            // enemy killed
            Debug.Log("Enemy hp left "+u.hp + " heal on kill "+healOnKills+ " healing " +heal+" dmg "+damage);
            if (u.dead && healOnKills > 0) {
                actionData.sourceExecutingUnit.Heal(healOnKills, this);
            }
        }
        if (data.standard.setStatus != CombatStatus.SameAsBefore)
            actionData.sourceExecutingUnit.combatStatus = data.standard.setStatus;
    }
}
