
using UnityEngine;


[System.Serializable]
public class MoveAttackData : AbilityEffect{
    public GridMask range;

    public bool onStartApplyAOE, onEndApplyAOE;
    public int[] endAnimSets;
    internal GridMask originalRange { get; private set; }
    public void SetRange(GridMask newRange) {
        if (originalRange == null)
            originalRange = range;
        range = newRange;
    }


    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onMove) {
            Execute(info);
            info.executingUnit.AbilitySuccess();
        }
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

