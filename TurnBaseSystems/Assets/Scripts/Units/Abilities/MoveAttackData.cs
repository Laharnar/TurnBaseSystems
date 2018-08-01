
using UnityEngine;

[System.Serializable]
public class PassiveData : AttackDataType {
    public bool canHeal = false;
    public int healAmt = 1;

    public bool canBackstab = false;
    public GridMask backstabRange;
    public int backstabDmg = 1;

    public void Execute(CurrentActionData a, AttackData2 ability) {
        if (canBackstab) {
            Unit[] units= ability.passive.backstabRange.GetUnits(a.attackStartedAt);
            // backstab 1 unit
            for (int i = 0; i < units.Length; i++) {
                if (units[i].flag.allianceId!=a.sourceExecutingUnit.flag.allianceId) {
                    units[i].GetDamaged(backstabDmg);
                    break;
                }
            }
        }
    }
}


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

