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
    public TargetFilter targetFilter;

    public float pierceRange=3f;

    public int maxCount = 1;
    public bool useChargesCountToRepeatPiercing = false;
    public bool restoreChargesUsedOnKills = false;

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onAttack) {
            Execute(info);
            info.executingUnit.AbilitySuccess();
        }
    }

    public void Execute(AbilityInfo info) {
        Unit firstHitUnit = GridAccess.GetUnitAtPos(info.attackedSlot);
        Unit[] unit = GetUnitsPierced(info.executingUnit, firstHitUnit);
        UnityEngine.Debug.Log(info.executingUnit +" found piercing units "+unit.Length);
        float reduction = bounceDamageReduction;
        int returnCharges = 0;
        for (int i = 0; i < unit.Length; i++) {
            if (unit[i] == null) {
                Debug.Log("Null unit for some reason.");
                continue;
            }
            if (useChargesCountToRepeatPiercing) {
                if (info.executingUnit.charges == 0)
                    break;
                info.executingUnit.AddCharges(null, -1);
            }
            info.attackedSlot = unit[i].snapPos;
            info.activeAbility.standard.Execute(info);
            // restore on killing units
            if (useChargesCountToRepeatPiercing
                && restoreChargesUsedOnKills && (unit[i]==null || unit[i].dead)) {
                returnCharges++;
            }
            StandardAttackData.dmgReduction += bounceDamageReduction;
        }
        if (returnCharges > 0)
            info.executingUnit.AddCharges(null, returnCharges);
        StandardAttackData.dmgReduction = 0f;
    }
    public void Draw(Unit firstHitUnit) {
        Debug.Log("[Draw]");
        Unit[] unit = GetUnitsPierced(PlayerTurnData.Instance.selectedPlayerUnit, firstHitUnit);
        for (int i = 0; i < unit.Length; i++) {
            GridDisplay.Instance.SetUpGrid(unit[i].snapPos, GridDisplayLayer.OrangePierce, GridMask.One);
        }
    }

    public Unit[] GetUnitsPierced(Unit executingUnit, Unit firstHitUnit) {
        Dictionary<Unit, Unit> foundUnits = new Dictionary<Unit, Unit>();
        if (!executingUnit) { UnityEngine.Debug.Log("No Attacking unit assigned to combat info"); return new Unit[0]; }
        int pierceCount = useChargesCountToRepeatPiercing ?
            executingUnit.charges : maxCount;
        UnityEngine.Debug.Log("Pierce attack with max pierce count "+pierceCount + " using charges "+ useChargesCountToRepeatPiercing + " Pierce range "+pierceRange);
        Unit lastPierce = firstHitUnit;
        Vector3 dirOfAtk = lastPierce.snapPos - executingUnit.snapPos;
        int x = 100;
        while (pierceCount > 0) {
            x--;
            if (x < 0) {
                Debug.Log("infi loop");
                break;
            }
            //Debug.Log(lastPierce.snapPos + " "+ executingUnit.snapPos +" "+ dirOfAtk + " "+ pierceRange);
            Unit[] units = SelectionManager.GetAllUnitsFromDirection(lastPierce.snapPos, dirOfAtk.normalized, pierceRange);
            bool noNewUnits = true;
            // save next bounce unit and resume search from it.
            foreach (var unit in units) {
                if (EmpowerAlliesData.ValidTarget(executingUnit, targetFilter, units[0].flag.allianceId)
                    && !foundUnits.ContainsKey(unit) && pierceCount > 0) {
                    Debug.Log("[PIERCE ability/search] found " + unit);
                    foundUnits.Add(unit, unit);
                    noNewUnits = false;
                    pierceCount--;
                }
            }
            if (noNewUnits) {
                break;
            }
            // continoue bouncing off the next unit
            if (units.Length > 0) {
                lastPierce = units[units.Length-1];
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
        Debug.Log("[UnDraw]");
        Unit[] unit = GetUnitsPierced(PlayerTurnData.Instance.selectedPlayerUnit, firstHitUnit);
        for (int i = 0; i < unit.Length; i++) {
            GridDisplay.Instance.HideGrid(unit[i].snapPos, GridDisplayLayer.OrangePierce, GridMask.One);
        }
    }
}