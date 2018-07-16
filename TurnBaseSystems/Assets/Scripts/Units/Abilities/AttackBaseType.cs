using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[System.Serializable]
public abstract class AttackBaseType {
    public abstract void ApplyDamage(Unit source, GridItem attackedSlot);
    
    public static AttackBaseType GetAttackType(AttackType atkType) {
        switch (atkType) {
            case AttackType.SingleTarget:
                return new RangedAttack();
            case AttackType.Aura:
                return new AoeMaskAttack();
            case AttackType.LongRangeAoe:// note: aoe mask attack is for cone attacks, not mouse
                return new AoeMaskAttack();
            case AttackType.Hunker:
                return new Hunker();
            case AttackType.Pickup:
                return new PickItem();
            case AttackType.ThrowEquipped:
                return new ThrowEquipped();
            case AttackType.EquippedWeapon:
                return new AttackWithEquipped();
            case AttackType.EnhanceWeapon:
                return new Enhance();
            case AttackType.Building:
                return new SlotBuilding();
            case AttackType.Deconstruct:
                return new SlotConsumption();
            case AttackType.GroundDrain:
                return new FloraDrain();
            case AttackType.UnitDrain:
                return new LifeDrain();
            case AttackType.RestoreAP:
                return new RestoreAP();
            default:
                break;
        }
        return new RangedAttack();
    }
    
}
