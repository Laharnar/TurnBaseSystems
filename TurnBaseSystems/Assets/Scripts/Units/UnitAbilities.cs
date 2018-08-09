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
        return data.ToArray();
    }
    
    public void ActivateOnSteppedOn(Unit unit, Unit steppedOnBy) {
        CI.curActivator.Reset();
        CI.curActivator.onStepOnEnemy = true;
        steppedOnBy.RunAllAbilities(CI.curActivator);
        unit.RunAllAbilities(CI.curActivator);

        for (int i = 0; i < abilityOnSteppedOn.Length; i++) {
            if (abilityOnSteppedOn[i] < additionalAbilities2.Count) {
                steppedOnBy.AttackAction2(unit.snapPos, additionalAbilities2[abilityOnSteppedOn[i]]);
            }
        }
    }
}

