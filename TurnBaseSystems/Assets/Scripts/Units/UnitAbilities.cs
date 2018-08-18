using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitAbilities : MonoBehaviour {
    public int[] abilityOnSteppedOn;// on collision, activate ability on self.
    public AttackData2 move2;

    public List<AttackData2> additionalAbilities2 = new List<AttackData2>();

    public AttackDataLib atkLib;
    public AnimDataHolder abilityAnimations;

    private void Awake() {
        if (abilityAnimations == null) {
            abilityAnimations = GetComponent<AnimDataHolder>();
        }
        if (atkLib == null) {
            atkLib = GetComponent<AttackDataLib>();
        }

    }

    public virtual AttackData2[] GetNormalAbilities() {
        List<AttackData2> data = new List<AttackData2>();
        data.Add(move2);
        data.AddRange(additionalAbilities2.ToArray());
        for (int i = 0; i < data.Count; i++) {
            if (data[i].active == false) {
                data.RemoveAt(i);
            }
        }
        return data.ToArray();
    }
    
    public void ActivateOnSteppedOn(Unit unit, Unit steppedOnBy) {
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onStepOnEnemy = true;
        steppedOnBy.RunAllAbilities(AbilityInfo.CurActivator);
        unit.RunAllAbilities(AbilityInfo.CurActivator);

        for (int i = 0; i < abilityOnSteppedOn.Length; i++) {
            if (abilityOnSteppedOn[i] < additionalAbilities2.Count) {
                PlayerTurnData.Instance.activeAbility = additionalAbilities2[abilityOnSteppedOn[i]];
                //int action = selectedPlayerUnit.AttackAction2(hoveredSlot, activeAbility);
                PlayerTurnData copy = PlayerTurnData.Instance.Copy();
                copy.selectedPlayerUnit = steppedOnBy;
                
                Combat.Instance.abilitiesQue.Enqueue(new AbilityInfo(copy.selectedPlayerUnit, copy.selectedAttackSlot, copy.activeAbility));

                //steppedOnBy.AttackAction2(unit.snapPos, additionalAbilities2[abilityOnSteppedOn[i]]);
            }
        }
    }
}

