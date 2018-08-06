using System;
using UnityEngine;

public enum AuraTarget {
    Allies,
    Enemies,
    Civilians,
    All
}
public enum AuraTrigger {
    OnUnitEntersExits,
    OnTurnEnd,
    OnAttacked,
}
[System.Serializable]
public class EmpowerAlliesData : AttackDataType {
    public GridMask auraRange;

    public int stdDmgUp;
    public int aoeDmgUp;
    public int shieldUp;
    public AuraTarget target;
    public AuraTrigger trigger;
    public int maxAuraStacks = 1;

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
            Effect(source, u);
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

    public int GetAuraStacks(Unit unit, CombatStatType statType) {
        return unit.stats.GetStatCountOfType<EmpowerAlliesData>(statType);
    }

    public void Effect(Unit source, Unit other) {
        if (ValidTarget(target, source.flag.allianceId, source)
            && (GetAuraStacks(other, CombatStatType.Armor) < maxAuraStacks)) {
            if (stdDmgUp != 0) other.stats.Increase(this, CombatStatType.StdDmg, stdDmgUp);
            if (aoeDmgUp != 0) other.stats.Increase(this, CombatStatType.AoeDmg, aoeDmgUp);
            if (shieldUp != 0) other.AddShield(this, shieldUp);
        }
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
        buff.setStatus = setStatus;
        buff.shieldUp = shieldUp;
        buff.stdDmgUp = stdDmgUp;
        buff.used = used;
        buff.auraRange = auraRange;
        return buff;
    }

    /// <summary>
    /// Activates all auras of certain type on certain unit.
    /// </summary>
    /// <param name="oldDeff"></param>
    /// <param name="newAff"></param>
    /// <param name="unit"></param>
    /// <param name="trigger"></param>
    internal static void DeffectEffect(Vector3 oldDeff, Vector3 newAff, Unit unit, AuraTrigger trigger) {
        // Note: this also applies, if aura was already consumed mid - turn, by counter

        Vector3 unit1Snap = GridManager.SnapPoint(unit.transform.position);
        foreach (var ability in unit.abilities.additionalAbilities2) {
            if (ability.aura.used && ability.aura.trigger == trigger) {
                bool inOld = ability.aura.auraRange.IsPosInMask(unit1Snap, oldDeff);
                bool inNew = ability.aura.auraRange.IsPosInMask(unit1Snap, newAff);
                ability.aura.DeEffectArea(oldDeff, unit, true);
                ability.aura.EffectArea(newAff, unit);
            }
        }
    }
}

public static class CombatInfo {
    public static Unit attackingUnit;
    public static CurrentActionData currentActionData;
    public static AttackData2 activeAbility;
}
