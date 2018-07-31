using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitAbilities : MonoBehaviour {
    public bool newVersion = false;
    public AttackData2 move2;

    public List<AttackData2> additionalAbilities2 = new List<AttackData2>();

    public AnimDataHolder abilityAnimations;

    private void Awake() {
        if (abilityAnimations == null) {
            abilityAnimations = GetComponent<AnimDataHolder>();
        }

        if (!newVersion) {
            Debug.Log("ReplaceWithNew version ", this);
        }
    }

    public virtual AttackData2[] GetNormalAbilities() {
        List<AttackData2> data = new List<AttackData2>();
        if (newVersion) {
            data.Add(move2);
            data.AddRange(additionalAbilities2.ToArray());
        } /*else {
            data.Add(move);
            data.AddRange(additionalAbilities.ToArray());
        }*/
        return data.ToArray();
    }
    
}

