
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

    internal override void AtkBehaviourExecute() {
        Execute();
    }

    public void Execute() {
        if (onStartApplyAOE) {
            CI.activeAbility.aoe.Execute();
        }
        Unit existing = GridAccess.GetUnitAtPos(CI.attackedSlot);
        
        CI.sourceExecutingUnit.MoveAction(CI.attackedSlot);

        if (onEndApplyAOE) {
            CI.activeAbility.aoe.Execute();
        }

        if (existing) {
            existing.abilities.ActivateOnSteppedOn(existing, CI.sourceExecutingUnit);
        }

        if (setStatus != CombatStatus.SameAsBefore)
            CI.sourceExecutingUnit.combatStatus = setStatus;
    }
}

