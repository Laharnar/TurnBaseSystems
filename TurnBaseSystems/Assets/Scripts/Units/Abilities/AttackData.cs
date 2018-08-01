using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StdAttackData {
}
/// <summary>
/// Attacks are manually coded into UnitAbilities-derived classes.
/// These are what is avaliable to units to execute.
/// Targets are units.
/// </summary>
[System.Serializable]
[System.Obsolete("Not actively used")]
public sealed class AttackData : StdAttackData {

    public string o_attackName;
    public GridMask attackMask;
    [System.Obsolete("Don't use")]
    public string attackType = "Normal";
    public int actionCost = 1;
    public bool requiresUnit = true;
    public AttackAnimationInfo animData;

    /// <summary>
    /// To change attack function by editor
    /// </summary>
    public AttackType attackType_EditorOnly;

    public AttackBaseType attackFunction;
    public AttackTypeData attackData;
    
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