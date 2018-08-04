using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows calculations and damage for pierce attack, after some source.
/// </summary>
[System.Serializable]
public class PierceAtkData:AttackDataType {
    public float bounceDamageReduction = 0f;

    public AuraTarget targetFilter;

    public float pierceRange=3f;

    public int maxCount = 1;
    public bool useChargesCountToRepeatPiercing = false;
    public bool restoreChargesUsedOnKills = false;

    public void Execute(Vector3 attackedSlot) {
        Unit firstHitUnit = GridAccess.GetUnitAtPos(attackedSlot);
        Unit[] unit = GetUnitsPierced(firstHitUnit);
        UnityEngine.Debug.Log("Found piercing units "+unit.Length);
        float reduction = bounceDamageReduction;
        for (int i = 0; i < unit.Length; i++) {
            if (unit[i] == null) {
                Debug.Log("Null unit for some reason.");
                continue;
            }
            if (CombatInfo.currentActionData == null) {
                Debug.Log("No combat info");
                continue;
            }

            if (useChargesCountToRepeatPiercing) {
                if (CombatInfo.attackingUnit.charges == 0)
                    break;
                CombatInfo.attackingUnit.AddCharges(this, -1);
            }
            CombatInfo.activeAbility.standard.Execute(new CurrentActionData() {
            attackedSlot = unit[i].snapPos, attackStartedAt = CombatInfo.currentActionData.attackStartedAt, sourceExecutingUnit = CombatInfo.attackingUnit });
            // restore on killing units
            if (useChargesCountToRepeatPiercing 
                && restoreChargesUsedOnKills && unit[i] == null) {
                CombatInfo.attackingUnit.AddCharges(this, 1);
            }
            StandardAttackData.dmgReduction += bounceDamageReduction;
        }
        StandardAttackData.dmgReduction = 0f;
    }
    public void Draw(Unit firstHitUnit) {
        Unit[] unit = GetUnitsPierced(firstHitUnit);
        for (int i = 0; i < unit.Length; i++) {
            GridDisplay.SetUpGrid(unit[i].snapPos, 1, 2, GridMask.One);
        }
    }

    public Unit[] GetUnitsPierced(Unit firstHitUnit) {
        Dictionary<Unit, Unit> foundUnits = new Dictionary<Unit, Unit>();
        if (!CombatInfo.attackingUnit) { UnityEngine.Debug.Log("No Attacking unit assigned to combat info"); return new Unit[0]; }
        int pierceCount = useChargesCountToRepeatPiercing ?
            CombatInfo.attackingUnit.charges : maxCount;
        UnityEngine.Debug.Log("Pierce attack with max pierce count "+pierceCount + " Pierce range "+pierceRange);
        Unit lastPierce = firstHitUnit;
        for (int i = 0; i < pierceCount; i++) {
            Unit[] units = SelectionManager.GetAllUnitsFromDirection(lastPierce.snapPos, CombatInfo.currentActionData.directionOfAttack.normalized, pierceRange);
            bool noNewUnits = true;
            for (int j = 0; j < units.Length; j++) {
                if (EmpowerAlliesData.ValidTarget(targetFilter, units[i].flag.allianceId, CombatInfo.attackingUnit)
                    && !foundUnits.ContainsKey(units[i])) {
                    foundUnits.Add(units[i], units[i]);
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
            GridDisplay.HideGrid(unit[i].snapPos, 2, GridMask.One);
        }
    }
}