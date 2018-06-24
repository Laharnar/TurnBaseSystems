using System;
using System.Collections.Generic;
using UnityEngine;
public static class AiHelper {


    public static GridMask FilterByInteractions(GridItem source, GridItem[] items, string attackType, GridMask template) {
        GridMask mask = template.EmptyCopy();
        for (int i = 0; i < items.Length; i++) {
            int x = items[i].gridX - source.gridX + mask.w / 2;// mask.LowerLeftFromX(source.gridX) + mask.LowerLeftFromX();
            int y = items[i].gridY - source.gridY + mask.l / 2;// mask.LowerLeftFromY(source.gridY) + mask.LowerLeftFromX();
            /*int indI =  - mask.w / 2;
            int indJ = j - mask.l / 2;
            /*if (gridX + indI > -1 && gridX + indI < m.width && gridY + indJ > -1 && gridY + indJ < m.length) {
                GridItem item = m.gridSlots.GetItem(gridX + indI, gridY + indJ);
                mask.Get[items[i].gridX].col[items[i].gridY] = GridItem.TypeFilter(items[i], attackType);
                if (mask.Get(i, j)) {
                */
            //if (x > -1 && x < GridManager.m.width && y > -1 && y < GridManager.m.length) {

                mask.mask[x].col[y] = GridItem.TypeFilter(items[i], attackType);
            //}
        }
        return mask;
    }

    public static float[] GetDistances<T>(this Vector3 source, T[] units) where T: MonoBehaviour {
        float[] distances = new float[units.Length];
        for (int i = 0; i < units.Length; i++) {
            distances[i] = Vector3.Distance(units[i].transform.position, source);
        }
        return distances;
    }

    public static int GetIndexOfMin(this float[] list) {
        float min = float.MaxValue;
        int minI = -1;
        for (int i = 0; i < list.Length; i++) {
            if (list[i] < min) {
                min = list[i];
                minI = i;
            }
        }
        return minI;
    }

    /// <summary>
    /// Example: Put range unit on max attack range.
    /// NOT 100% reliable.
    /// </summary>
    /// <param name="curSlot"></param>
    /// <param name="target"></param>
    /// <param name="moveMask"></param>
    /// <param name="attackMask"></param>
    /// <returns></returns>
    internal static GridItem ClosestToAttackEdgeOverMoveMask(GridItem curSlot, GridItem target, GridMask moveMask, GridMask attackMask) {
        // gets slot where unit should be moved to still be in attack range
        GridItem optimalMovePos = ClosestFreeSlotOnEdge(curSlot.transform.position, target, attackMask);
        GridItem viable = FurthestFreeSlotOnEdge(curSlot, target, moveMask);
        List<GridItem> list = new List<GridItem> { optimalMovePos, viable };
        list = FilterByMask(list, curSlot, moveMask);
        if (list.Count == 2)
            list = new List<GridItem>() { optimalMovePos };
        float[] dists = GetDistances<GridItem>(target.transform.position, list.ToArray());
        return list[GetIndexOfMin(dists)]; 
    }

    public static int GetIndexOfMax(this float[] list) {
        float max = float.MinValue;
        int maxI = -1;
        for (int i = 0; i < list.Length; i++) {
            if (list[i] > max) {
                max = list[i];
                maxI = i;
            }
        }
        return maxI;
    }


    /// <summary>
    /// Pick a slot that's furthest away from mask source, and closest to target.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are applying mask</param>
    /// <returns></returns>
    public static GridItem FurthestFreeSlotOnEdge(GridItem maskSource, GridItem targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position - maskSource.transform.position);
        dir.Normalize();
        GridItem[] nbrs = GridManager.GetSlotsInMask(maskSource.gridX, maskSource.gridY, mask);
        float[] distsToTarget = GetDistances<GridItem>(targetSlot.transform.position + dir, nbrs);
        float[] distsToSource = GetDistances<GridItem>(maskSource.transform.position - dir, nbrs);
        // Closest slot in max range is the slot with minimum summed distance.
        // Slot that also on edge, is furthest away from mask and closest to source
        int index = 0;
        float minSum = float.MaxValue;
        float minDistFromTarget = float.MaxValue;
        float maxDistFromSource = float.MinValue;
        for (int i = 0; i < distsToTarget.Length; i++) {
            if (distsToTarget[i] + distsToSource[i] <= minSum
                && distsToTarget[i] < minDistFromTarget && distsToSource[i] > maxDistFromSource) {
                minSum = distsToTarget[i] + distsToSource[i];
                minDistFromTarget = distsToTarget[i];
                maxDistFromSource = distsToSource[i];
                index = i;
            }
        }
        return nbrs[index];
    }

    /// <summary>
    /// Gets slot with shortest distance of all slots in range, on edge of mask.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are applying mask</param>
    /// <returns></returns>
    public static GridItem ClosestFreeSlotOnEdge(Vector3 pos, GridItem targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position- pos);
        dir.Normalize();
        GridItem[] nbrs = GridManager.GetSlotsInMask(targetSlot.gridX, targetSlot.gridY, mask);
        float[] distsToTarget = GetDistances<GridItem>(targetSlot.transform.position + dir, nbrs);
        float[] distsToSource = GetDistances<GridItem>(pos - dir, nbrs);
        // Closest slot in max range is the slot with minimum summed distance.
        // Slot that also on edge, is furthest away from mask and closest to source
        int index = 0;
        float minSum = float.MaxValue;
        float maxDistFromTarget = float.MinValue;
        float minDistFromSource = float.MaxValue;
        for (int i = 0; i < distsToTarget.Length; i++) {
            if (distsToTarget[i] + distsToSource[i] <= minSum
                && distsToTarget[i] > maxDistFromTarget && distsToSource[i] < minDistFromSource) {
                minSum = distsToTarget[i] + distsToSource[i];
                maxDistFromTarget = distsToTarget[i];
                minDistFromSource = distsToSource[i];
                index = i;
            }
        }
        return nbrs[index];
    }

    /// <summary>
    /// Gets slot with shortest distance of all slots in range, on edge of mask.
    /// Prefers opposing side(Just reverse +- dir).
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static GridItem ClosestFreeSlotOnOppositeEdge(Vector3 pos, GridItem targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position- pos).normalized;
        GridItem[] nbrs = GridManager.GetSlotsInMask(targetSlot.gridX, targetSlot.gridY, mask);
        float[] distsToTarget = GetDistances<GridItem>(targetSlot.transform.position -dir, nbrs);
        float[] distsToSource = GetDistances<GridItem>(pos + dir, nbrs);
        // Closest slot in max range is the slot with minimum summed distance.
        // Slot that also on edge, is furthest away from mask and closest to source
        int index = 0;
        float minSum = float.MaxValue;
        float maxDistFromTarget = float.MinValue;
        float minDistFromSource = float.MaxValue;
        for (int i = 0; i < distsToTarget.Length; i++) {
            if (distsToTarget[i] + distsToSource[i] <= minSum
                && distsToTarget[i] > maxDistFromTarget && distsToSource[i] < minDistFromSource) {
                minSum = distsToTarget[i] + distsToSource[i];
                maxDistFromTarget = distsToTarget[i];
                minDistFromSource = distsToSource[i];
                index = i;
            }
        }
        return nbrs[index];
    }


    /// <summary>
    /// Gets slot with shortest distance of all slots in range.
    /// NOT RELIABLE to return edge. Use closestfreeslotonedge instead.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static GridItem ClosestFreeSlotStillInMask(Vector3 pos, GridItem targetSlot, GridMask mask) {

        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position- pos).normalized;
        GridItem[] nbrs = GridManager.GetSlotsInMask(targetSlot.gridX, targetSlot.gridY, mask);
        float[] distsToTarget = GetDistances<GridItem>(targetSlot.transform.position - dir, nbrs);
        float[] distsToSource = GetDistances<GridItem>(pos + dir, nbrs);
        // Closest slot in max range is the slot with minimum summed distance.
        int index = 0;
        float minSum = float.MaxValue;
        for (int i = 0; i < distsToTarget.Length; i++) {
            if (distsToTarget[i]+distsToSource[i] < minSum) {
                minSum = distsToTarget[i] + distsToSource[i];
                index = i;
            }
        }
        return nbrs[index];
    }
    /// <summary>
    /// Gets closest slot next to target slot.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static GridItem ClosestSlotToSlot(Vector3 pos, GridItem targetSlot) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position- pos).normalized;
        List<GridItem> nbrs = Neighbours(targetSlot);
        float[] dists = GetDistances<GridItem>(targetSlot.transform.position-dir, nbrs.ToArray());
        return nbrs[dists.GetIndexOfMin()];
    }
    
    /// <summary>
    /// Gets closest slot next to target slot.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static GridItem ClosestFreeSlotToSlot(Vector3 pos, GridItem targetSlot) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (targetSlot.transform.position- pos).normalized;
        List<GridItem> nbrs = Neighbours(targetSlot);
        for (int i = 0; i < nbrs.Count; i++) {
            if (!nbrs[i].Walkable) {
                nbrs.RemoveAt(i);
                i--;
                continue;
            }
        }
        if (nbrs.Count == 0) {
            return null;
        }
        float[] dists = GetDistances<GridItem>(targetSlot.transform.position - dir, nbrs.ToArray());
        return nbrs[dists.GetIndexOfMin()];
    }

    internal static GridItem ClosestToTargetOverMask(GridItem source, GridItem targetSlot, GridMask mask) {
        List<GridItem> nbrs = Neighbours(targetSlot);
        nbrs.Add(FurthestFreeSlotOnEdge(source, targetSlot, mask));
        nbrs = FilterByMask(nbrs, source, mask);
        if (nbrs.Count == 0) {
            Debug.Log("no moves");
            return null;
        }
        Vector3 dir = (targetSlot.transform.position- source.transform.position).normalized;
        float[] dists = GetDistances<GridItem>(targetSlot.transform.position - dir, nbrs.ToArray());
        return nbrs[dists.GetIndexOfMin()];
    }


    public static List<GridItem> FilterByMask(List<GridItem> items, GridItem source, GridMask mask) {
        for (int i = 0; i < items.Count; i++) {
            if (!GridManager.IsSlotInMask(source, items[i], mask)) {
                items.RemoveAt(i);
                i--;
            }
        }
        return items;
    }

    public static bool IsNeighbour(GridItem slot, GridItem other) {
        List<GridItem> slots = new List<GridItem>();
        if (slot.gridX > 0) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX - 1, slot.gridY));
        }
        if (slot.gridY > 0) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX, slot.gridY - 1));
        }
        if (slot.gridX + 1 < GridManager.m.width) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX + 1, slot.gridY));
        }
        if (slot.gridY + 1 > GridManager.m.length) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX, slot.gridY + 1));
        }
        return slots.Contains(other);
    }

    public static List<GridItem> Neighbours(GridItem slot) {
        List<GridItem> slots = new List<GridItem>();
        if (slot.gridX > 0) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX - 1, slot.gridY));
        }
        if (slot.gridY > 0) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX, slot.gridY - 1));
        }
        if (slot.gridX+1 < GridManager.m.width) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX+1, slot.gridY));
        }
        if (slot.gridY+1 < GridManager.m.length) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX, slot.gridY+1));
        }
        return slots;
    }
}
