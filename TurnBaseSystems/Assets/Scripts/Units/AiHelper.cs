using System.Collections.Generic;
using UnityEngine;
public static class AiHelper { 

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
    /// Gets slot with shortest distance of all slots in range, on edge of mask.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
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
    /// Not reliable to get the one on outer side.
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
        if (slot.gridY+1 > GridManager.m.length) {
            slots.Add(GridManager.m.gridSlots.GetItem(slot.gridX, slot.gridY+1));
        }
        return slots;
    }
}
