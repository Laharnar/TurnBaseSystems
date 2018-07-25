using System;
using System.Collections.Generic;
using UnityEngine;
public class GridDisplay {
    static List<Grid> allGrids = new List<Grid>();
    static List<Unit> allUnits = new List<Unit>();
    static Vector3 tmpGridPos;
    static Grid tmpGrid;

    internal static void TmpDisplayGrid(Vector3 vector3, int v, GridMask mask) {
        Vector3 position = GridManager.SnapPoint(vector3, true);
        tmpGridPos = position;
        if (tmpGrid != null)
            TmpHideGrid(vector3, mask);
        if (mask != null) {
            Grid g = GridManager.NewGridInstance(position, mask);
            g.ShowColor(v);
            tmpGrid = g;
        }
    }

    internal static void TmpHideGrid(Vector3 vector3, GridMask mask) {
        //Vector3 position = GridManager.SnapPoint(vector3);
        if (tmpGrid != null) {
            tmpGrid.HalfRemove(null);
            tmpGrid = null;
        }
    }

    public static void DisplayGrid(Unit unit, int v, params GridMask[] mask) {
        Vector3 position = GridManager.SnapPoint(unit.transform.position);
        for (int i = 0; i < mask.Length; i++) {
            if (mask[i] != null) {
                Grid g = GridManager.NewGridInstance(GridManager.SnapPoint(position), mask[i]);
                g.ShowColor(v);
                allGrids.Add(g);
                allUnits.Add(unit);
            }
        }
    }

    public static void HideGrid(Unit unit, params GridMask[] mask) {
        if (unit == null) {
            Debug.Log("Null unit.");
            return;
        }
        for (int i = 0; i < allGrids.Count; i++) {
            if (allGrids[i] != null) {
                if (allUnits[i] == unit) {
                    allGrids[i].HalfRemove(null);
                    allGrids.RemoveAt(i);
                    allUnits.RemoveAt(i);
                    i--;
                }
            }
        }
        /*Debug.Log("visuals disabled 1");
        return;
        for (int i = 0; i < mask.Length; i++) {
            if (grids.ContainsKey(unit) && grids[unit] != null)
                grids[unit].ShowColor(0);
        }*/
    }
}

