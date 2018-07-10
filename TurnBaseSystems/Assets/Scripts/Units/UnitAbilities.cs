using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : AttackBaseType {
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (source.CanMoveTo(attackedSlot))
            source.MoveAction(attackedSlot);
    }
}
public abstract class UnitAbilities : MonoBehaviour {
    public AttackData move;
    public abstract AttackData BasicAttack { get; }
    public abstract GridMask BasicMask { get; }
    public AttackData MoveAction { get { return move; } }

    public List<AttackData> additionalAbilities = new List<AttackData>();

    public abstract AttackData[] GetNormalAbilities();

    protected AttackData[] AddAbilities(AttackData[] data) {
        
        List<AttackData> d = new List<AttackData>();
        
        d.Add(move);
        d.AddRange(additionalAbilities);
        d.AddRange(data);
        return d.ToArray();
    }
}

