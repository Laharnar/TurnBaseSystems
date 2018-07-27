using UnityEngine;

public enum AuraTarget {
    Allies,
    Enemies,
    Civilians,
    All
}
[System.Serializable]
public class EmpowerAlliesData : AttackDataType {
    public GridMask auraRange;

    public int stdDmgUp;
    public int aoeDmgUp;
    public int shieldUp;
    public AuraTarget target; 

    bool ValidTarget(AuraTarget target, int flag) {
        switch (target) {
            case AuraTarget.Allies:
                return flag == 0;
            case AuraTarget.Enemies:
                return flag == 1;
            case AuraTarget.Civilians:
                return false;
            case AuraTarget.All:
                return true;
            default:
                return false;
                break;
        }
    }

    public void EffectArea(UnityEngine.Vector3 pos) {
        Vector3[] unitsPos = auraRange.GetTakenPositions(pos);
        for (int i = 0; i < unitsPos.Length; i++) {
            Unit u = GridAccess.GetUnitAtPos(unitsPos[i]);
            int unitAlliance = u.flag.allianceId;
            if (ValidTarget(target, unitAlliance))
                Effect(u);
        }
    }
    public void DeEffectArea(UnityEngine.Vector3 pos) {
        Vector3[] unitsPos = auraRange.GetTakenPositions(pos);
        for (int i = 0; i < unitsPos.Length; i++) {
            Unit u = GridAccess.GetUnitAtPos(unitsPos[i]);
            int unitAlliance = u.flag.allianceId;
            if (ValidTarget(target, unitAlliance))
                LoseEffect(u);
        }
    }
    public void Effect(Unit u) {
        if (stdDmgUp != 0) u.stats.Increase(this, CombatStatType.StdDmg, stdDmgUp);
        if (aoeDmgUp != 0) u.stats.Increase(this, CombatStatType.AoeDmg, aoeDmgUp);
        if (shieldUp!= 0) u.AddShield(this, shieldUp);
    }

    public void LoseEffect(Unit u) {
        if (stdDmgUp != 0) u.stats.Reduce(this, CombatStatType.StdDmg, stdDmgUp);
        if (aoeDmgUp != 0) u.stats.Reduce(this, CombatStatType.AoeDmg, aoeDmgUp);
        if (shieldUp != 0) u.AddShield(this, -shieldUp);
    }

    internal EmpowerAlliesData Copy() {
        EmpowerAlliesData buff = new EmpowerAlliesData();
        buff.animSets = animSets;
        buff.aoeDmgUp= aoeDmgUp;
        buff.auraRange = auraRange;
        buff.priority = priority;
        buff.setStatus = setStatus;
        buff.shieldUp = shieldUp;
        buff.stdDmgUp = stdDmgUp;
        buff.used = used;
        buff.auraRange = auraRange;
        return buff;
    }
}
