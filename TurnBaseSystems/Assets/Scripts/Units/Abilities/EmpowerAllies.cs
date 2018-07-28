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

    public static bool ValidTarget(AuraTarget target, int flag, Unit source) {
        switch (target) {
            case AuraTarget.Allies:
                return flag == source.flag.allianceId;
            case AuraTarget.Enemies:
                return flag != source.flag.allianceId;
            case AuraTarget.Civilians:
                return false;
            case AuraTarget.All:
                return true;
            default:
                return false;
                break;
        }
    }

    public void EffectArea(UnityEngine.Vector3 pos, Unit source) {
        Vector3[] unitsPos = auraRange.GetTakenPositions(pos);
        for (int i = 0; i < unitsPos.Length; i++) {
            Unit u = GridAccess.GetUnitAtPos(unitsPos[i]);
            int unitAlliance = u.flag.allianceId;
            if (ValidTarget(target, unitAlliance, source))
                Effect(u);
        }
    }
    public void DeEffectArea(UnityEngine.Vector3 pos, Unit source, bool alwaysDeEffectUnit) {
        Vector3[] unitsPos = auraRange.GetTakenPositions(pos);
        bool sourceDeEffected = false;
        for (int i = 0; i < unitsPos.Length; i++) {
            Unit u = GridAccess.GetUnitAtPos(unitsPos[i]);
            int unitAlliance = u.flag.allianceId;
            if (ValidTarget(target, unitAlliance, source)) {
                LoseEffect(u);
                if (u==source) {
                    sourceDeEffected = true;
                }
            }
        }
        if (alwaysDeEffectUnit && !sourceDeEffected) {
            LoseEffect(source);
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

    public void BeforePosChange() {
        // lose effect

    }
    public void AfterPosChange() {
        // re effect

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
