
using UnityEngine;


[System.Serializable]
public class MoveAttackData : AbilityEffect{
    public GridMask range;

    public bool onStartApplyAOE, onEndApplyAOE;
    public int[] endAnimSets;

    internal MoveAttackData Copy() {
        MoveAttackData move = new MoveAttackData();
        move.endAnimSets = endAnimSets;
        move.onStartApplyAOE = onStartApplyAOE;
        move.onEndApplyAOE = onEndApplyAOE;
        move.range = range;
        return move;
    }

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onMove)
            Execute(info);
    }

    public void Execute(AbilityInfo info) {
        if (onStartApplyAOE) {
            info.activeAbility.aoe.Execute(info);
        }
        Unit existing = GridAccess.GetUnitAtPos(info.attackedSlot);

        info.executingUnit.MoveAction(info.attackedSlot);

        if (onEndApplyAOE) {
            info.activeAbility.aoe.Execute(info);
        }

        if (existing) {
            existing.abilities.ActivateOnSteppedOn(existing, info.executingUnit);
        }

        if (setStatus != CombatStatus.SameAsBefore)
            info.executingUnit.combatStatus = setStatus;
    }
}

