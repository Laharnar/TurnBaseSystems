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
public class AbilityEffect{
    internal static DamageInfo curDmg;
    public bool used = false;
    public int[] animSets;
    public CombatStatus setStatus = CombatStatus.Normal;
    public CombatEventMask activator;


    internal virtual void AtkBehaviourExecute(AbilityInfo info) {
        
    }

    internal void Copy(AbilityEffect copyTo) {
        copyTo.used = used;
        copyTo.animSets = animSets;
        copyTo.setStatus = setStatus;
        copyTo.activator = activator;
    }
}
