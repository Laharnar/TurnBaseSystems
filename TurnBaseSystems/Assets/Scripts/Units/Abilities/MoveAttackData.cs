
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
            AbilityInfo.ActiveAbility.aoe.Execute();
        }
        Unit existing = GridAccess.GetUnitAtPos(AbilityInfo.AttackedSlot);
        
        AbilityInfo.SourceExecutingUnit.MoveAction(AbilityInfo.AttackedSlot);

        if (onEndApplyAOE) {
            AbilityInfo.ActiveAbility.aoe.Execute();
        }

        if (existing) {
            existing.abilities.ActivateOnSteppedOn(existing, AbilityInfo.SourceExecutingUnit);
        }

        if (setStatus != CombatStatus.SameAsBefore)
            AbilityInfo.SourceExecutingUnit.combatStatus = setStatus;
    }
}

