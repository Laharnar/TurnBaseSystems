using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BUFFAttackData : AbilityEffect {
    public bool activated = true;
    public int turns = 1;
    public int armorAmt;
    public int recieveDmg;
    public float dmgMultiplierUp;
    public int healAmount;
    public int addCounters = 0;
    public bool spawnOnDeath = false;
    public int buffedTransformsOnDeath;
    public GridMask temporaryMoveRange;
    public GridMask temporaryAttackRange;
    public BuffType buffType = BuffType.None;
    public CombatStatus endBuffStatus = CombatStatus.Normal;
    public int[] endAnimSets;
    
    internal BUFFAttackData Copy() {
        BUFFAttackData buff = new BUFFAttackData();
        buff.activated = activated;
        buff.turns = turns;
        buff.buffType = buffType;
        buff.armorAmt = armorAmt;
        buff.healAmount = healAmount;
        buff.recieveDmg = recieveDmg;
        buff.dmgMultiplierUp = dmgMultiplierUp;
        buff.endBuffStatus = endBuffStatus;
        buff.endAnimSets = endAnimSets;
        buff.addCounters = addCounters;
        buff.spawnOnDeath = spawnOnDeath;
        buff.buffedTransformsOnDeath = buffedTransformsOnDeath;
        buff.temporaryAttackRange = temporaryAttackRange;
        buff.temporaryMoveRange = temporaryMoveRange;
        base.Copy(buff);
        return buff;
    }

    /*internal void Register(Unit source, Unit target) {
        BuffManager.Register(source, target, this);
        ExecuteOnStart(target, this);
    }*/

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        CombatEventMask mask = info.activator;
        //BUFFAttackData buffInstance = AbilityInfo.ActiveBuffData.buff;
        Debug.Log("Buff activation attempt activators-atk:"+mask.onAttack + (" turnEnd:"+mask.onAnyTurnEnd+" "+ (AbilityInfo.ActiveOrigBuff!=null)+" "+ (AbilityInfo.ActiveBuffData!=null)) +" death:"+mask.onUnitDies);
        if (mask.onAttack) {
            // original is this, when attacking
            ExecuteOnStart(info.TargetedUnit, this);
            BuffManager.Register(info.executingUnit, info.TargetedUnit, this);
            info.executingUnit.AbilitySuccess();
        }
        if (mask.onAnyTurnEnd) {
            // original is now saved in combat info
            // additional check for buff-tick.
            if (AbilityInfo.ActiveOrigBuff!= null && AbilityInfo.ActiveBuffData!=null) {
                Consume(AbilityInfo.ActiveOrigBuff, AbilityInfo.ActiveBuffData);
            }
        }
        if (mask.onUnitDies) {
            info.executingUnit.AbilitySuccess();
            UnitDeath();
            BuffManager.Remove(AbilityInfo.ActiveOrigBuff, this, AbilityInfo.ActiveBuffData);
        }
    }
    internal void Execute(Unit target) {
        if (healAmount != 0) {
            Debug.Log("[heal buff] +hp" + healAmount + " t:"+target);
            target.Heal(healAmount, null);
        }
        if (recieveDmg!= 0) {
            Debug.Log("[bleed] get dmg " + recieveDmg);
            target.GetDamaged(recieveDmg);
        }
        if (setStatus != CombatStatus.SameAsBefore) {
            Debug.Log(target.combatStatus +"->"+ setStatus);
            target.combatStatus = setStatus;
        }
    }

    internal void Consume(BUFFAttackData origBuff, BuffUnitData data) {
        turns--;
        Debug.Log(data.source+ "->"+data.target+" Consumed buff tick ."+(turns+1)+"->"+turns);
        // activate buff
        if (turns <= 0) {
            ExecuteOnEnd(data.target, origBuff);
            Debug.Log("Ending buff " + GetType() + " " + setStatus + " s:" + data.source + " t:" + data.target);
            BuffManager.Remove(origBuff, this, data);
        } else {
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
        if (temporaryMoveRange != null) {
            Debug.Log("[move range buff] +range" + " t:" + target);
            target.abilities.move2.standard.SetRange(temporaryMoveRange);
        }
        if (temporaryAttackRange != null) {
            Debug.Log("[move range buff] +range" + " t:" + target);
            target.abilities.move2.standard.SetRange(temporaryAttackRange);
        }
        if (dmgMultiplierUp != 0) {
            Debug.Log("[dmg buff] +dmg mult" + dmgMultiplierUp + " t:" + target);
            target.doDmgMult += dmgMultiplierUp;
        }
        if (addCounters != 0) {
            Debug.Log("[counter] +counter count" + addCounters + " t:" + target);
            target.reflectDmgTimes += addCounters;
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
        if (temporaryMoveRange != null) {
            Debug.Log("[move range buff] -range"  + " t:" + target);
            target.abilities.move2.move.SetRange(target.abilities.move2.move.originalRange);
        }
        if (dmgMultiplierUp != 0) {
            Debug.Log("[dmg buff] -dmg mult" + dmgMultiplierUp + " t:" + target);
            target.doDmgMult -= dmgMultiplierUp;
        }
        if (addCounters != 0) {
            Debug.Log("[counter] -counter count" + addCounters + " t:" + target);
            target.reflectDmgTimes -= addCounters;
        }
        if (endBuffStatus != CombatStatus.SameAsBefore)
            target.combatStatus = endBuffStatus;
    }
    public void UnitDeath() {
        
    }
}