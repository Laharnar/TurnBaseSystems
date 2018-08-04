using System;
using UnityEngine;

[System.Serializable]
public class AOEAttackData : AttackDataType {
    public int damage=1;
    public GridMask aoeMask;

    public bool useChargesToChooseLimitSlots = false;
    public AuraTarget targets = AuraTarget.All;
    public DamageInfo damageInfo = new DamageInfo(1, DamageType.Magic, EnergyType.None, DamageAttribute.HardObject);

    internal void Execute(CurrentActionData a, AttackData2 data) {
        curDmg = damageInfo;
        GridMask mask = GridMask.RotateMask(/*data.aoe.*/aoeMask, PlayerFlag.m.mouseDirection);
        Unit[] vec = mask.GetUnits(a.attackedSlot);
        int charges = CombatInfo.attackingUnit.charges;
        Unit source = CombatInfo.attackingUnit;
        for (int i = 0; i < vec.Length; i++) {
            if (!vec[i] || !EmpowerAlliesData.ValidTarget(targets, vec[i].flag.allianceId, source)) continue;
            if (useChargesToChooseLimitSlots) {
                if (charges > 0) {
                    charges--;
                    source.AddCharges(this, -1);
                } else {
                    break;
                }
            }
            vec[i].GetDamaged(data.aoe.damage);//data.aoe.damage);
        }
        if (/*data.aoe.*/setStatus != CombatStatus.SameAsBefore)
            a.sourceExecutingUnit.combatStatus = setStatus;//data.aoe.setStatus;
    }
}
