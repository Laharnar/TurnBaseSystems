using System;
using UnityEngine;

public enum AttackRequirments {
    Any,
    RequiresUnit,
    RequiresEmpty
}
[System.Serializable]
public sealed class AttackData2 : StdAttackData {
    public string o_attackName;
    public string detailedDescription;
    public int actionCost = 1;
    public AttackRequirments requirements = AttackRequirments.Any;
    public StandardAttackData standard;
    public AOEAttackData aoe;
    public BUFFAttackData buff;
    public EmpowerAlliesData aura;
    public MoveAttackData move;

    public GridMask AttackMask {
        get {
            if (standard.used) {
                return standard.attackRangeMask;
            }
            return null;
        }
    }

    public static void ShowGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.standard.used) {
            GridDisplay.DisplayGrid(source, 2, data.standard.attackRangeMask);
        }
        if (data.move.used) {
            GridDisplay.DisplayGrid(source, 1, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.TmpDisplayGrid(0,curSlot, 4, data.aoe.aoeMask);
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.TmpDisplayGrid(1,attackedSlot, 4, data.aoe.aoeMask);
            }
        }else if (data.aoe.used) {
            GridDisplay.TmpDisplayGrid(2,attackedSlot, 4, data.aoe.aoeMask);
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        
    }
    public static void HideGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.standard.used) {
            GridDisplay.HideGrid(source, data.standard.attackRangeMask);
        }
        if (data.move.used) {
            GridDisplay.HideGrid(source, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.TmpHideGrid(0,curSlot, data.aoe.aoeMask);
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.TmpHideGrid(1,attackedSlot, data.aoe.aoeMask);
            }
        } else if (data.aoe.used) {
            GridDisplay.TmpHideGrid(2,attackedSlot, data.aoe.aoeMask);
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }

    }

    /// <summary>
    /// Executes attack data.
    /// Included all types of attacks(normal, aoe, buff...)
    /// Modifies Unit.combatStatus
    /// </summary>
    /// <param name="source"></param>
    /// <param name="attackedSlot"></param>
    /// <param name="data"></param>
    public static void UseAttack(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (source == null || data==null) {
            Debug.Log("Error. source:" + (source == null) + " slot:" + attackedSlot + " data:"+(data == null));
        }

        Debug.Log("Using attack "+data.o_attackName +" unit: "+source+
            " "+data.standard.used+" "+data.aoe.used+" "+ data.buff.used);
        //AttackDataType[] attacks = data.GetAttacks();

        CurrentActionData actionData = new CurrentActionData() {
            attackedSlot = attackedSlot,
            attackStartedAt = source.snapPos,
            sourceExecutingUnit = source };
        // standard
        if (data.standard.used) {
            Unit u = GridAccess.GetUnitAtPos(attackedSlot);
            if (u) {
                u.GetDamaged(data.standard.damage);
            }
            if (data.standard.setStatus!= CombatStatus.SameAsBefore)
            source.combatStatus = data.standard.setStatus;
        }
        // aoe
        if (data.aoe.used) {
            data.aoe.Execute(actionData, data);
        }
        // buff
        if (data.buff.used) {
            ActivateBuff(source, data.buff);

            BuffManager.Register(source, data.buff);
            if (data.buff.setStatus!= CombatStatus.SameAsBefore)
            source.combatStatus = data.buff.setStatus;
        }
        if (data.move.used) {
            source.MoveAction(attackedSlot, data);

            if (data.move.setStatus != CombatStatus.SameAsBefore)
                source.combatStatus = data.move.setStatus;
        }
    }

    private static void ActivateBuff(Unit source, BUFFAttackData buff) {
        if (buff.buffType == BuffType.Shielded) {
            source.AddShield(buff, buff.armorAmt);
        }
    }

    /// <summary>
    /// Activates triggers and bools in anim controller.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="triggers"></param>
    public static void RunAnimations(Unit unit, int[] triggers) {
        unit.abilities.abilityAnimations.Run(unit, triggers);
    }

    public static float AnimLength(Unit unit, AttackData2 attack) {
        float maxLen = 0;
        float f1 = attack.standard.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.standard.animSets) : 0,
            f2 = attack.aoe.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.aoe.animSets) : 0,
            f3 = attack.buff.used ? AnimDataHolder.GetLongestTriggerAnimLength(unit, attack.buff.animSets) : 0;
        if (attack.standard.used) maxLen = maxLen < f1 ? f1 : maxLen;
        if (attack.aoe.used) maxLen = maxLen < f2 ? f2 : maxLen;
        if (attack.buff.used) maxLen = maxLen < f3 ? f3 : maxLen;
        // todo: for dash
        return maxLen;
    }
}
