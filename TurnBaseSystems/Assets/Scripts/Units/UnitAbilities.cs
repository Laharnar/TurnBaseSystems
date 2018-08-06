using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitAbilities : MonoBehaviour {
    public AttackData2 move2;

    public List<AttackData2> additionalAbilities2 = new List<AttackData2>();

    public AnimDataHolder abilityAnimations;

    private void Awake() {
        if (abilityAnimations == null) {
            abilityAnimations = GetComponent<AnimDataHolder>();
        }

    }

    public virtual AttackData2[] GetNormalAbilities() {
        List<AttackData2> data = new List<AttackData2>();
        data.Add(move2);
        data.AddRange(additionalAbilities2.ToArray());
        return data.ToArray();
    }
    
}

