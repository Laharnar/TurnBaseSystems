using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager m { get; private set; }

    public Grid<GridItem> gridSlots;

    public int width, length;

    public Vector2 itemDimensions;
    public Transform pref;
    public Transform gridParent;
    public Transform rootLoader;

    private void Awake() {
        m = this;

        gridSlots = new Grid<GridItem>(width, length, rootLoader);
        //UpdateGrid();
    }

    [ContextMenu("Update grid")]
    public void UpdateGrid () {
        if (gridSlots == null)
            gridSlots = new Grid<GridItem>(width, length);
        gridSlots.InitGrid(gridParent.transform.position, itemDimensions, pref, gridParent);
    }

    public static bool AreSlotsInRange(GridItem curSlot, GridItem attackedSlot, int range) {
        return Vector3.Distance(curSlot.transform.position, attackedSlot.transform.position)
            <= range*Mathf.Max(m.itemDimensions.x, m.itemDimensions.y);
    }

    internal static GridItem[] GetSlotsInInteractiveRange(Unit slot, GridMask mask) {
        if (mask == null) // return all
        {
            return m.gridSlots.AsArray();
        }
        return GetSlotsInMask(slot.gridX, slot.gridY, mask);
    }

    public static void RecolorRange(int color, params GridItem[] slots) {
        for (int i = 0; i < slots.Length; i++) {
            slots[i].RecolorSlot(color);
        }
    }

    public static void RecolorMask(GridItem u, int color, GridMask attackMask) {
        RecolorRange(color, GetSlotsInMask(u.gridX, u.gridY, attackMask));
    }

    public static GridItem[] GetSlotsInRange(int gridX, int gridY, int range) {
        List<GridItem> items = new List<GridItem>();
        for (int i = gridX-range; i < gridX+range+1; i++) {
            for (int j = gridY-range; j < gridY+range+1; j++) {
                if (i > -1 && i < m.width && j > -1 && j < m.length) {
                    GridItem item = m.gridSlots.GetItem(i, j);
                    if (AreSlotsInRange(m.gridSlots.GetItem(gridX, gridY), item, range)) {
                        items.Add(item);
                    }
                }
            }
        }
        return items.ToArray();
    }

    /// <summary>
    /// Checks if target is in mask of source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsSlotInMask(GridItem source, GridItem target, GridMask mask) {
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask isn't defined");
            return false;
        }
        int i = (target.gridX - source.gridX)+mask.w/2;
        int j = (target.gridY - source.gridY)+mask.l/2;
        if (i > -1 && i < mask.w && j > -1 && j < mask.l) {
            return mask.Get(i,j);
        }
        return false;
    }

    public static GridItem[] GetSlotsInMask(int gridX, int gridY, GridMask mask) {
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask isn't defined");
            return null;
        }
        List<GridItem> items = new List<GridItem>();
        for (int i =  0; i < mask.w; i++) {
            for (int j =  0; j < mask.l; j++) {
                int indI = i - mask.w / 2;
                int indJ = j - mask.l / 2;
                if (gridX + indI > -1 && gridX + indI < m.width && gridY + indJ > -1 && gridY + indJ < m.length) {
                    GridItem item = m.gridSlots.GetItem(gridX + indI, gridY +indJ);
                    if (mask.Get(i,j)) {
                        items.Add(item);
                    }
                }
            }
        }
        return items.ToArray();
    }
}
