using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
/// <summary>
/// Attacks are manually coded into UnitAbilities-derived classes.
/// These are what is avaliable to units to execute.
/// Targets are units.
/// </summary>
[System.Serializable]
public sealed class AttackData {
    public string o_attackName;
    public GridMask attackMask;
    public string attackType = "Normal";
    public int actionCost = 1;
    public bool requiresUnit = true;
    public AttackAnimationInfo animData;

    /// <summary>
    /// To change attack function by editor
    /// </summary>
    public AttackType attackType_EditorOnly;

    public AttackBaseType attackFunction;

    public void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackFunction == null) {
            attackFunction = AttackBaseType.GetAttackType(attackType_EditorOnly);
            Debug.Log("Auto assigning attack "+o_attackName+"as "+attackType_EditorOnly.ToString() + " "+source, source);
        }
        if (attackFunction != null)
            attackFunction.ApplyDamage(source, attackedSlot);
        else Debug.Log("No attack function defined");
    }

}

public enum AttackType {
    SingleTarget,
    Aura,
    LongRangeAoe,
    SelfTarget,
    Pickup,
    Building,
    GroundDrain,
    UnitDrain,
    Hunker,
    EquippedWeapon,
    EnhanceWeapon,
    PassEquipped,
    ThrowEquipped,
    Deconstruct,
    RestoreAP
}

/// <summary>
/// Target is interactible envirounment.
/// Will automatically scan for interactive items in range.
/// <seealso cref="InteractiveEnvirounment"/>
/// </summary>
[System.Serializable]
public abstract class EnvirounmentalAttack:AttackBaseType {

}