using System;
using UnityEngine;

public enum AttackRequirments {
    Any,
    RequiresUnit,
    RequiresEmpty
}
[System.Serializable]
public sealed class AttackData2 : StdAttackData {
    public bool active = true;

    public string o_attackName;
    public string detailedDescription;
    public int actionCost = 1;
    public AttackRequirments requirements = AttackRequirments.Any;
    public AttackRangeData range;
    public StandardAttackData standard;
    public AOEAttackData aoe;
    public BUFFAttackData buff;
    public EmpowerAlliesData aura;
    public MoveAttackData move;
    public PassiveData passive;
    public PierceAtkData pierce;
    
    public static void ShowGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.move.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked!= null) {
            data.pierce.Draw(attacked);
        }
    }
    public static void HideGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null)
            return;
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.move.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked != null) {
            data.pierce.Hide(attacked);
        }
    }
    public static void HideRotatedGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null)
            return;
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.move.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked != null) {
            data.pierce.Hide(attacked);
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
        if (data.active == false) {
            Debug.Log("Error, activating disabled ability. source:" + (source == null) + " slot:" + attackedSlot + " data:" + (data == null));
            return;
        }

        Debug.Log("Using attack "+data.o_attackName +" unit: "+source+
            " "+data.standard.used+" "+data.aoe.used+" "+ data.buff.used);
        //AttackDataType[] attacks = data.GetAttacks();

        CurrentActionData actionData = new CurrentActionData() {
            attackedSlot = attackedSlot,
            attackStartedAt = source.snapPos,
            sourceExecutingUnit = source };
        CombatInfo.currentActionData = actionData;
        CombatInfo.activeAbility = data;
        // standard
        if (data.standard.used) {
            data.standard.Execute(actionData);
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
            data.move.Execute(actionData, data);
            //source.MoveAction(attackedSlot, data);

            if (data.move.setStatus != CombatStatus.SameAsBefore)
                source.combatStatus = data.move.setStatus;
        }
        if (data.pierce.used) {
            data.pierce.Execute(attackedSlot);
        }
        if (data.passive.used) {
            data.passive.Execute(new CurrentActionData() { attackedSlot = attackedSlot, attackStartedAt = attackedSlot, sourceExecutingUnit = source }, data);
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
