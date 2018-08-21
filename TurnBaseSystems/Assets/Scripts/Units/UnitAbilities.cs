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

        int counter = 0;
        if (move2.active) {
            move2.id = 0;
            counter++;
        }
        for (int i = 0; i < additionalAbilities2.Count; i++) {
            if (additionalAbilities2[i].active) {
                additionalAbilities2[i].id = counter;
                counter++;
            }
        }
    }

    public virtual AttackData2[] GetNormalAbilities() {
        List<AttackData2> data = new List<AttackData2>();
        data.Add(move2);
        data.AddRange(additionalAbilities2.ToArray());
        for (int i = 0; i < data.Count; i++) {
            if (data[i].active == false) {
                data.RemoveAt(i);
                i--;
            }
        }
        return data.ToArray();
    }
    
    public void ActivateOnSteppedOn(Unit unit, Unit steppedOnBy) {
        CombatEvents.DebugEvents("OnSteppedOn-not fixed");
        AbilityInfo.CurActivator.Reset();
        AbilityInfo.CurActivator.onStepOnEnemy = true;
        //steppedOnBy.RunAllAbilities(AbilityInfo.CurActivator);
        //unit.RunAllAbilities(AbilityInfo.CurActivator);

        //steppedOnBy.RunAllAbilities2(AbilityInfo.CurActivator);
        //unit.RunAllAbilities2(AbilityInfo.CurActivator);

        for (int i = 0; i < abilityOnSteppedOn.Length; i++) {
            if (abilityOnSteppedOn[i] < additionalAbilities2.Count) {
                
                Combat.RegisterAbilityUse(unit, steppedOnBy.snapPos, additionalAbilities2[abilityOnSteppedOn[i]]);

                //steppedOnBy.AttackAction2(unit.snapPos, additionalAbilities2[abilityOnSteppedOn[i]]);
            }
        }
    }
}

