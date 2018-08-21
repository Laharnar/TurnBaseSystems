using System;
using UnityEngine;

// Note: assumes source ability stats don't change.
public class AbilityInfo {
    public static AbilityInfo Instance;
    public Unit executingUnit;
    public Vector3 attackStartedAt;
    public Vector3 attackedSlot;
    public AttackData2 activeAbility;
    public CombatEventMask activator;

    public Unit TargetedUnit { get { return GridAccess.GetUnitAtPos(attackedSlot); } }


    /// <summary>
    /// For activating in middle of player turn.
    /// </summary>
    /// <param name="instance"></param>
    public AbilityInfo(PlayerTurnData instance) {
        executingUnit = instance.selectedPlayerUnit;
        attackStartedAt = instance.selectedPlayerUnit.snapPos;
        attackedSlot = instance.hoveredSlot;
        activeAbility = instance.activeAbility;
    }

    public AbilityInfo(Unit executingUnit, Vector3 attackedSlot, AttackData2 activeAbility) {
        this.executingUnit= executingUnit;
        if (executingUnit)
            attackStartedAt = this.executingUnit.snapPos;
        this.attackedSlot = attackedSlot;
        this.activeAbility = activeAbility;
    }


    public AbilityInfo(Unit executingUnit, Vector3 attackedSlot, AttackData2 activeAbility, CombatEventMask mask) {
        this.executingUnit = executingUnit;
        if (executingUnit)
            attackStartedAt = this.executingUnit.snapPos;
        this.attackedSlot = attackedSlot;
        this.activeAbility = activeAbility;
        this.activator = mask;
    }
    // todo
    public static AttackData2 ActiveAbility { get { return PlayerTurnData.ActiveAbility; } set { PlayerTurnData.Instance.activeAbility = value; } }
    public static CombatEventMask CurActivator { get { return Instance.activator; } set { Instance.activator = value; } }
    public static BUFFAttackData ActiveOrigBuff;
    public static BuffUnitData ActiveBuffData;

    public static Unit ExecutingUnit { get { return Instance.executingUnit; } set { Instance.executingUnit = value; } }
    public static Vector3 AttackStartedAt { get { return Instance.attackStartedAt; } set { Instance.attackStartedAt = value; } }
    public static Vector3 AttackedSlot { get { return Instance.attackedSlot; } set { Instance.attackedSlot = value; } }

    public static Vector3 directionOfAttack { get { return AttackedSlot - AttackStartedAt; } }

    internal void Reset() {
        executingUnit = null;
        activeAbility = null;
        ActiveAbility = null;
        executingUnit = null;
        attackStartedAt = new Vector3(10000,0,0);
        attackedSlot = new Vector3(10000,0,0);
        activator.Reset();
    }
}
