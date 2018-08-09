using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BUFFAttackData : AbilityEffect {
    public bool activated = true;
    public int turns = 1;
    public int armorAmt;
    public int healAmount;
    public BuffType buffType = BuffType.None;
    public CombatStatus endBuffStatus = CombatStatus.Normal;
    public int[] endAnimSets;
    
    internal BUFFAttackData Copy() {
        BUFFAttackData buff = new BUFFAttackData();
        buff.turns = turns;
        buff.buffType = buffType;
        buff.armorAmt = armorAmt;
        buff.healAmount = healAmount;
        buff.endBuffStatus = endBuffStatus;
        buff.endAnimSets = endAnimSets;
        return buff;
    }

    internal void Register(Unit source, Unit target) {
        BuffManager.Register(source, target, this);
        ExecuteOnStart(target, this);
    }

    internal override void AtkBehaviourExecute() {
        CombatEventMask mask = CI.curActivator;
        BUFFAttackData buffInstance = CI.activeBuffData.buff;
        if (mask.onAttack) {
            // original is this, when attacking
            ExecuteOnStart(CI.sourceExecutingUnit, this);
            BuffManager.Register(CI.sourceExecutingUnit, CI.sourceSecondaryExecUnit, this);
        }
        if (mask.onAnyBuffTick) {
            // original is now saved in combat info
            if (CI.sourceExecutingUnit.flag.allianceId != CI.sourceSecondaryExecUnit.flag.allianceId) {
                Consume(CI.activeOrigBuff, CI.activeBuffData);
            }
        }
        if (mask.onUnitDies) {
            UnitDeath();
            BuffManager.Remove(CI.activeOrigBuff, this, CI.activeBuffData);
        }
    }
    internal void Execute(Unit target) {
        AttackData2.RunAnimations(target, endAnimSets);
        
        if (healAmount != 0) {
            Debug.Log("[heal buff] +hp" + healAmount + " t:"+target);
            target.Heal(healAmount, null);
        }
        if (setStatus != CombatStatus.SameAsBefore)
            target.combatStatus = setStatus;
    }

    internal void Consume(BUFFAttackData origBuff, BuffUnitData data) {
        turns--;
        
        // activate buff
        if (turns <= 0) {
            ExecuteOnEnd(data.target, origBuff);
            Debug.Log("Ending buff " + GetType() + " " + setStatus + " s:" + data.source + " t:" + data.target);
            BuffManager.Remove(origBuff, this, data);
        } else {
            Debug.Log("[heal buff] +hp" + healAmount + " t:"+data.target);
            Execute(data.target);
        }
    }

    void ExecuteOnStart(Unit target, BUFFAttackData original) {
        if (armorAmt != 0) {
            Debug.Log("[shield buff] +shield " + armorAmt);
            target.AddShield(original, armorAmt);
        }
        if (healAmount != 0) {
            Debug.Log("[heal buff] +hp" + healAmount + " t:" + target);
            target.Heal(healAmount, null);
        }
        if (setStatus != CombatStatus.SameAsBefore)
            target.combatStatus = setStatus;
    }
    private void ExecuteOnEnd(Unit target, BUFFAttackData original) {
        AttackData2.RunAnimations(target, endAnimSets);
        if (armorAmt != 0) {
            Debug.Log("[shield buff] -shield " + armorAmt);
            target.AddShield(original, -armorAmt);
        }
        if (setStatus != CombatStatus.SameAsBefore)
            target.combatStatus = setStatus;
    }
    public void UnitDeath() {

    }
}

