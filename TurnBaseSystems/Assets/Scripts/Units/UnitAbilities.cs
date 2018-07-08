using System;
using System.Collections.Generic;
using UnityEngine;
public abstract class UnitAbilities : MonoBehaviour {
    public abstract AttackData BasicAttack { get; }
    public abstract GridMask BasicMask { get; }
    public List<AttackData> additionalAbilities = new List<AttackData>();

    public abstract AttackData[] GetNormalAbilities();

    protected AttackData[] AddAbilities(AttackData[] data) {
        List<AttackData> d = new List<AttackData>();
        d.AddRange(additionalAbilities);
        d.AddRange(data);
        return d.ToArray();
    }
}

