using System;

public enum DamageType {
    Psyhical,
    Magic,
}
public enum EnergyType {
    None,
    Inner,
    Outer
}
public enum DamageAttribute {
    HardObject,
    Combustion,
    Solidifaction,
    Shockwave,
}
[System.Serializable]
public class AttackDataType {
    internal static DamageInfo curDmg;
    public bool used = false;
    public int[] animSets;
    public CombatStatus setStatus = CombatStatus.Normal;

}
