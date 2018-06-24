using System;

/// <summary>
/// Attacks are manually coded into UnitAbilities-derived classes.
/// These are what is avaliable to units to execute.
/// Targets are units.
/// </summary>
[System.Serializable]
public abstract class Attack {
    public GridMask attackMask;
    public string attackType = "Normal";

    public abstract void ApplyDamage(Unit source, GridItem attackedSlot);
}

/// <summary>
/// Target is interactible envirounment.
/// Will automatically scan for interactive items in range.
/// <seealso cref="InteractiveEnvirounment"/>
/// </summary>
[System.Serializable]
public abstract class EnvirounmentalAttack:Attack {

}