using System;
using System.Collections.Generic;
using UnityEngine;
public static class GridAccess {

    public static Unit[] OnlyAlliedUnits(Vector3[] filter, int allianceId) {
        Combat.Instance.UnitNullCheck();
        List<Unit> items = new List<Unit>();
        Unit[] units = Combat.Instance.units.ToArray();
        Vector3[] snapped = new Vector3[units.Length];
        for (int i = 0; i < units.Length; i++) {
            snapped[i] = units[i].snapPos;
        }
        for (int i = 0; i < filter.Length; i++) {
            for (int j = 0; j < snapped.Length; j++) {
                if (filter[i] == snapped[j] && units[j].flag.allianceId == allianceId) {
                    items.Add(units[j]);
                }
            }
        }
        return items.ToArray();
    }

    internal static Unit[] OnlyHostileUnits(Vector3[] filter, int skippedAllianceId) {
        List<Unit> items = new List<Unit>();
        Unit[] units = Combat.Instance.units.ToArray();
        Vector3[] snapped = new Vector3[units.Length];
        for (int i = 0; i < units.Length; i++) {
            snapped[i] = GridManager.SnapPoint(units[i].transform.position);
        }
        for (int i = 0; i < filter.Length; i++) {
            for (int j = 0; j < snapped.Length; j++) {
                if (filter[i] == snapped[j] && units[i].flag.allianceId != skippedAllianceId) {
                    items.Add(units[i]);
                }
            }
        }
        return items.ToArray();
    }


    internal static Unit GetUnitAtPos(Vector3 slot) {
        Combat.Instance.UnitNullCheck();
        for (int i = 0; i < Combat.Instance.units.Count; i++) {
            if (Combat.Instance.units[i].snapPos == slot) {
                return Combat.Instance.units[i];
            }
        }
        return null;
    }
    /// <summary>
    /// For multi unit per slot system.
    /// Atm, limits to 1.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    internal static ISlotItem[] GetItemsAtPos(Vector3 pos) {
        Unit u = GetUnitAtPos(pos);
        return u != null ? new ISlotItem[] { u } : null;
    }
}