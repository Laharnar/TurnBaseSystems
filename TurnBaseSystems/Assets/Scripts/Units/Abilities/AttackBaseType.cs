using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class AttackBaseType {
    public abstract void ApplyDamage(Unit source, GridItem attackedSlot);
    
    public static AttackBaseType GetAttackType(AttackType atkType) {
        
        return new RangedAttack();
    }
    
}
