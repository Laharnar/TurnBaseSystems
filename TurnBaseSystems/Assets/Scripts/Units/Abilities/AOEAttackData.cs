using System;
using UnityEngine;

[System.Serializable]
public class AOEAttackData : AttackDataType {
    public int damage=1;
    public GridMask aoeMask;

    internal void Execute(CurrentActionData a, AttackData2 data) {
        GridMask mask = GridMask.RotateMask(/*data.aoe.*/aoeMask, PlayerFlag.m.mouseDirection);
        Vector3[] vec = mask.GetPositions(a.attackedSlot);
        for (int i = 0; i < vec.Length; i++) {
            Unit u = GridAccess.GetUnitAtPos(vec[i]);
            if (u)
                u.GetDamaged(data.aoe.damage);
        }
        if (/*data.aoe.*/setStatus != CombatStatus.SameAsBefore)
            a.sourceExecutingUnit.combatStatus = setStatus;//data.aoe.setStatus;
    }
}
