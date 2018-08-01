
using UnityEngine;


[System.Serializable]
public class MoveAttackData : AttackDataType{
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

    public void Execute(CurrentActionData a, AttackData2 ability) {
        if (onStartApplyAOE) {
            ability.aoe.Execute(a, ability);
        }

        a.sourceExecutingUnit.MoveAction(a.attackedSlot, ability);

        if (onEndApplyAOE) {
            ability.aoe.Execute(a, ability);
        }

        if (setStatus != CombatStatus.SameAsBefore)
            a.sourceExecutingUnit.combatStatus = setStatus;
    }
}

