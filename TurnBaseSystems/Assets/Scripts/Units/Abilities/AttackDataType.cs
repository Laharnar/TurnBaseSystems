using System;

[System.Serializable]
public class AttackDataType {
    public bool used = false;
    public int priority = -1;
    public int[] animSets;
    public CombatStatus setStatus = CombatStatus.Normal;

}
