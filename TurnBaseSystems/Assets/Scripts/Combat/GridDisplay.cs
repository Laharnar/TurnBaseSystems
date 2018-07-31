using System;
using System.Collections.Generic;
using UnityEngine;
public class GridDisplay {
    static List<Grid> allGrids = new List<Grid>();
    static List<Unit> allUnits = new List<Unit>();
    static Vector3[] tmpGridPos = new Vector3[3];
    static Grid[] tmpGrid = new Grid[3];

    internal static void TmpDisplayGrid(int id, Vector3 vector3, int v, GridMask mask) {
        Vector3 position = GridManager.SnapPoint(vector3, true);
        tmpGridPos[id] = position;
        if (tmpGrid[id] != null)
            TmpHideGrid(id , vector3, mask);
        if (mask != null) {
            Grid g = GridManager.NewGridInstance(position, mask);
            g.ShowColor(v);
            tmpGrid[id] = g;
        }
    }

    internal static void TmpHideGrid(int id, Vector3 vector3, GridMask mask) {
        //Vector3 position = GridManager.SnapPoint(vector3);
        if (tmpGrid[id] != null) {
            tmpGrid[id].HalfRemove(null);
            tmpGrid[id] = null;
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
            Debug.Log("Hide grid/ Null unit.");
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
    }
}

