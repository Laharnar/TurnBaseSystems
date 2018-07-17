using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class GridManager : MonoBehaviour {
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

    private void Start() {
        Weapon.AssignAllDroppedWeaponsToSlots();
    }

    [ContextMenu("Update grid")]
    public void UpdateGrid () {
        if (gridSlots == null)
            gridSlots = new Grid<GridItem>(width, length);
        gridSlots.InitGrid(gridParent.transform.position, itemDimensions, pref, gridParent);
    }

    public static GridItem SnapToGrid(Vector3 point) {
        Vector3 o = point;
        point = point - m.gridParent.transform.position + (Vector3)m.itemDimensions / 2;
        point.z = 0;
        point.x = point.x - point.x % m.itemDimensions.x;
        point.y = point.y - point.y % m.itemDimensions.y;
        //point = new Vector2(point.y, point.x);
        GridItem[,] slots = m.gridSlots.data;
        for (int i = 0; i < m.width; i++) {
            for (int j = 0; j < m.length; j++) {
                if (new Vector3(i, j, 0) == point) {
                    Debug.Log("found: "+i+" "+j + " "+ slots[i, j].transform.position);
                    return slots[i, j];
                }
            }
        }
        Debug.Log(o+" "+point + " "+m.itemDimensions+"Doesn't match any slot.");
        return null;
    }

    public static void RecolorMask(GridItem u, int color, GridMask attackMask) {
        if (attackMask) {
            GridItem[] slots = GridAccess.GetSlotsInMask(u.gridX, u.gridY, attackMask);
            for (int i = 0; i < slots.Length; i++) {
                slots[i].RecolorSlot(color);
            }
        } else Debug.Log("Warning: mask is not assigned.");
    }
}

public static class GridLookup {

    /// <summary>
    /// Checks if target is in mask of source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsSlotInMask(GridItem source, GridItem target, GridMask mask) {
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask width or height is 0.");
            return false;
        }
        int i = (target.gridX - source.gridX) + mask.w / 2;
        int j = (target.gridY - source.gridY) + mask.l / 2;
        if (i > -1 && i < mask.w && j > -1 && j < mask.l) {
            return mask.Get(i, j);
        }
        return false;
    }


    public static bool AreSlotsInRange(GridItem curSlot, GridItem attackedSlot, int range) {
        return Vector3.Distance(curSlot.transform.position, attackedSlot.transform.position)
            <= range * Mathf.Max(GridManager.m.itemDimensions.x, GridManager.m.itemDimensions.y);
    }
    
}

public static class GridAccess {


    public static GridItem GetItem(int x, int y) {
        return GridManager.m.gridSlots.GetItem(x, y);
    }

    public static GridItem[] LoadLocalAoeAttackLayer(GridItem targetedSlot, GridMask aoeMask, int mouseDirection) {
        GridMask curFilter = aoeMask;
        if (curFilter.rotateable)
            curFilter = GridMask.RotateMask(curFilter, mouseDirection);
        GridItem[] items = GetSlotsInMask(targetedSlot.gridX, targetedSlot.gridY, curFilter);
        return items;
    }

    public static GridMask LoadAttackLayer(GridItem slot, AttackData curAttack, int mouseDirection) {
        return LoadMaskByInteractionType(slot, curAttack.attackMask, mouseDirection, curAttack.attackType);
    }

    /// <summary>
    /// Finds slots in area that fit filtering parameters
    /// </summary>
    /// <param name="curAttack"></param>
    public static GridMask LoadMaskByInteractionType(GridItem slot, GridMask mask, int mouseDirection, string attackType) {
        GridMask curFilter = mask;
        curFilter = GridMask.RotateMask(curFilter, mouseDirection);
        GridItem[] items = GetSlotsInMask(slot.gridX, slot.gridY, curFilter);
        return AiHelper.FilterByInteractions(slot, items, attackType, curFilter);
    }

    public static GridItem[] GetSlotsInMask(int gridX, int gridY, GridMask mask, OffsetMask offset) {
        if (offset != null) {
            offset.ApplyOffset(ref gridX, ref gridY);
        } else {
            Debug.Log("Warning: offset mask is null, normal GetSlotsInMask result.");
        }
        return GetSlotsInMask(gridX, gridY, mask);
    }

    public static GridItem[] GetSlotsInMask(int gridX, int gridY, GridMask mask) {
        if (mask == null) // return all
        {
            Debug.Log("Mask is null, returning all slots.");
            return GridManager.m.gridSlots.AsArray();
        }
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask width OR length is 0, returning null");
            return null;
        }
        GridItem center = GetItem(gridX, gridY);
        List<GridItem> items = new List<GridItem>();
        for (int i = 0; i < mask.w; i++) {
            for (int j = 0; j < mask.l; j++) {
                int indI = i - mask.w / 2;
                int indJ = j - mask.l / 2;
                if (gridX + indI > -1 && gridX + indI < GridManager.m.width && gridY + indJ > -1 && gridY + indJ < GridManager.m.length) {
                    GridItem item = GetItem(gridX + indI, gridY + indJ);
                    if (mask.Get(i, j)) {
                        items.Add(item);
                    }
                }
            }
        }
        return items.ToArray();
    }

    internal static GridItem[] OnlyUnits(GridItem[] filter) {
        List<GridItem> items = new List<GridItem>();
        for (int i = 0; i < filter.Length; i++) {
            if (filter[i].filledBy) {
                items.Add(filter[i]);
            }
        }
        return items.ToArray();
    }

    internal static GridItem[] GetSlotsInMask(GridItem curSlot, GridMask mask) {
        return GetSlotsInMask(curSlot.gridX, curSlot.gridY, mask);
    }

    internal static GridItem[] OnlyAlliedUnits(GridItem[] filter, int skippedAllianceId) {
        List<GridItem> items = new List<GridItem>();
        for (int i = 0; i < filter.Length; i++) {
            if (filter[i].filledBy && filter[i].filledBy.flag.allianceId == skippedAllianceId) {
                items.Add(filter[i]);
            }
        }
        return items.ToArray();
    }

    internal static GridItem[] OnlyHostileUnits(GridItem[] filter, int skippedAllianceId) {
        List<GridItem> items = new List<GridItem>();
        for (int i = 0; i < filter.Length; i++) {
            if (filter[i].filledBy && filter[i].filledBy.flag.allianceId != skippedAllianceId) {
                items.Add(filter[i]);
            }
        }
        return items.ToArray();
    }
}