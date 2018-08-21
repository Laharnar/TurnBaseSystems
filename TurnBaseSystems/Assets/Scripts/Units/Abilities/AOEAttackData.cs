using System;
using UnityEngine;

[System.Serializable]
public class AOEAttackData : DamageBasedAttackData {
    public int damage=1;
    public GridMask aoeMask;

    public bool useChargesToChooseLimitSlots = false;
    public AuraTarget targets = AuraTarget.All;
    public DamageInfo damageInfo = new DamageInfo(1, DamageType.Magic, EnergyType.None, DamageAttribute.HardObject);

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info != null && info.activator!= null && info.activator.onAttack)
            Execute(info);
    }

    internal void Execute(AbilityInfo info) {
        curDmg = damageInfo;
        GridMask mask = GridMask.RotateMask(/*data.aoe.*/aoeMask, 0);
        Unit[] vec = mask.GetUnits(info.attackedSlot);
        int charges = info.executingUnit.charges;
        Unit source = info.executingUnit;
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
            vec[i].GetDamaged(damage);//data.aoe.damage);
        }
        if (/*data.aoe.*/setStatus != CombatStatus.SameAsBefore)
            info.executingUnit.combatStatus = setStatus;//data.aoe.setStatus;
    }

    internal GridMask GetMask(int mouseDirection) {
        return GridMask.RotateMask(aoeMask, mouseDirection);
    }
}
