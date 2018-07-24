using System.Collections.Generic;
using UnityEngine;
public class GridDisplay {
    static Dictionary<Unit, Grid> grids = new Dictionary<Unit, Grid>();
    static List<Grid> allGrids = new List<Grid>();

    public static void DisplayGrid(Vector3 position, int v, GridMask curAoeFilter) {
        Grid g = GridManager.NewGridInstance(position, curAoeFilter);
        g.ShowColor(v);
        allGrids.Add(g);
    }

    public static void DisplayGrid(Unit unit, int v, GridMask mask) {
        Grid g = GridManager.NewGridInstance(unit.curSlot.worldPosition, mask);
        g.ShowColor(v);
        grids.Add(unit, g);
        allGrids.Add(g);
    }

    public static void HideGrid(Unit unit, GridMask mask) {
        if (grids.ContainsKey(unit) && grids[unit]!= null)
            grids[unit].ShowColor(0);
    }
}

