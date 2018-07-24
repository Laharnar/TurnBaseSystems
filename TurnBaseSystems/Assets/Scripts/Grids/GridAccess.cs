using System;
using System.Collections.Generic;
using UnityEngine;
public static class GridAccess {

    /*
    public static GridItem GetItem(int x, int y) {
        return GridManager.m.gridSlots.GetItem(x, y);
    }
    
    public static GridItem[] LoadLocalAoeAttackLayer(GridItem targetedSlot, GridMask aoeMask, int mouseDirection) {
        GridMask curFilter = aoeMask;
        curFilter = GridMask.RotateMask(curFilter, mouseDirection);
        GridItem[] items = GetSlotsInMask(targetedSlot.gridX, targetedSlot.gridY, curFilter);
        return items;
    }

    public static GridMask LoadAttackLayer(GridItem slot, GridMask attackMask,  int mouseDirection) {
        GridMask curFilter = attackMask;
        curFilter = GridMask.RotateMask(curFilter, mouseDirection);
        GridItem[] items = GetSlotsInMask(slot.gridX, slot.gridY, curFilter);
        return AiHelper.MaskFromItems(slot, items, curFilter);
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
    */
}