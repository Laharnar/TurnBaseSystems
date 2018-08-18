using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows calculations and damage for pierce attack, after some source.
/// </summary>
[System.Serializable]
public class PierceAtkData: DamageBasedAttackData {
    public float bounceDamageReduction = 0f;

    [Header("Who to dmg, allies still extend range")]
    public AuraTarget targetFilter;

    public float pierceRange=3f;

    public int maxCount = 1;
    public bool useChargesCountToRepeatPiercing = false;
    public bool restoreChargesUsedOnKills = false;

    internal override void AtkBehaviourExecute() {
        Execute();
    }

    public void Execute() {
        Unit firstHitUnit = GridAccess.GetUnitAtPos(AbilityInfo.AttackedSlot);
        Unit[] unit = GetUnitsPierced(firstHitUnit);
        UnityEngine.Debug.Log("Found piercing units "+unit.Length);
        float reduction = bounceDamageReduction;
        for (int i = 0; i < unit.Length; i++) {
            if (unit[i] == null) {
                Debug.Log("Null unit for some reason.");
                continue;
            }
            if (EmpowerAlliesData.ValidTarget(targetFilter, unit[i].flag.allianceId, AbilityInfo.SourceExecutingUnit)) {
                if (useChargesCountToRepeatPiercing) {
                    if (AbilityInfo.SourceExecutingUnit.charges == 0)
                        break;
                    AbilityInfo.SourceExecutingUnit.AddCharges(null, -1);
                }

                AbilityInfo.ActiveAbility.standard.Execute();
                // restore on killing units
                if (useChargesCountToRepeatPiercing
                    && restoreChargesUsedOnKills && (unit[i]==null || unit[i].dead)) {
                    AbilityInfo.SourceExecutingUnit.AddCharges(null, 1);
                }
            }
            StandardAttackData.dmgReduction += bounceDamageReduction;
        }
        StandardAttackData.dmgReduction = 0f;
    }
    public void Draw(Unit firstHitUnit) {
        Unit[] unit = GetUnitsPierced(firstHitUnit);
        for (int i = 0; i < unit.Length; i++) {
            GridDisplay.Instance.SetUpGrid(unit[i].snapPos, GridDisplayLayer.OrangePierce, GridMask.One);
        }
    }

    public Unit[] GetUnitsPierced(Unit firstHitUnit) {
        Dictionary<Unit, Unit> foundUnits = new Dictionary<Unit, Unit>();
        if (!AbilityInfo.SourceExecutingUnit) { UnityEngine.Debug.Log("No Attacking unit assigned to combat info"); return new Unit[0]; }
        int pierceCount = useChargesCountToRepeatPiercing ?
            AbilityInfo.SourceExecutingUnit.charges : maxCount;
        UnityEngine.Debug.Log("Pierce attack with max pierce count "+pierceCount + " using charges "+ useChargesCountToRepeatPiercing + " Pierce range "+pierceRange);
        Unit lastPierce = firstHitUnit;
        for (int i = 0; i < pierceCount; i++) {
            Unit[] units = SelectionManager.GetAllUnitsFromDirection(lastPierce.snapPos, AbilityInfo.directionOfAttack.normalized, pierceRange);
            bool noNewUnits = true;
            for (int j = 0; j < units.Length; j++) {
                if (units[j] == null) {
                    Debug.LogError("Null unit for some reason.");
                    continue;
                }
                if (EmpowerAlliesData.ValidTarget(targetFilter, units[j].flag.allianceId, AbilityInfo.SourceExecutingUnit)
                    && !foundUnits.ContainsKey(units[j])) {
                    foundUnits.Add(units[j], units[j]);
                    noNewUnits = false;
                    break;
                }
            }
            if (noNewUnits) {
                break;
            }
        }
        Unit[] r = new Unit[foundUnits.Keys.Count];
        int ii = 0;
        foreach (var item in foundUnits.Keys) {
            r[ii] = item;
            ii++;
        }
        return r;
    }

    internal void Hide(Unit firstHitUnit) {
        Unit[] unit = GetUnitsPierced(firstHitUnit);
        for (int i = 0; i < unit.Length; i++) {
            GridDisplay.Instance.HideGrid(unit[i].snapPos, GridDisplayLayer.OrangePierce, GridMask.One);
        }
    }
}