using System.Collections.Generic;
using UnityEngine;
public static class AiHelper { 

    public static float[] GetDistances<T>(this Vector3 source, List<T> units) where T: MonoBehaviour {
        float[] distances = new float[units.Count];
        for (int i = 0; i < units.Count; i++) {
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
    /// Gets closest slot next to target slot.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static GridItem ClosestSlotToSlot(Vector3 pos, GridItem targetSlot) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3 dir = (pos - targetSlot.transform.position).normalized;
        List<GridItem> nbrs = Neighbours(targetSlot);
        float[] dists = GetDistances<GridItem>(targetSlot.transform.position+dir, nbrs);
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
        Vector3 dir = (pos - targetSlot.transform.position).normalized;
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
        float[] dists = GetDistances<GridItem>(targetSlot.transform.position + dir, nbrs);
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
