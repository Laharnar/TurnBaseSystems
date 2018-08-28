using System;
using System.Collections.Generic;
using UnityEngine;
public static class AiHelper {
    /*public static GridMask MaskFromItems(GridItem source, GridItem[] items, GridMask template) {
        if (template == null) {
            Debug.Log("Error, template is null.");
            return null;
        }
        GridMask mask = template.EmptyCopy();
        for (int i = 0; i < items.Length; i++) {
            int x = items[i].gridX - source.gridX + mask.w / 2;
            int y = items[i].gridY - source.gridY + mask.l / 2;
            mask.mask[x].col[y] = items[i];
        }
        return mask;
    }*/
    public static float[] GetDistances(this Vector3 source, Unit[] positions) {
        float[] distances = new float[positions.Length];
        for (int i = 0; i < positions.Length; i++) {
            distances[i] = Vector3.Distance(GridManager.SnapPoint(positions[i].transform.position), source);
        }
        return distances;
    }
    public static float[] GetDistances(this Vector3 source, Vector3[] positions) {
        float[] distances = new float[positions.Length];
        for (int i = 0; i < positions.Length; i++) {
            distances[i] = Vector3.Distance(positions[i], source);
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

    internal static Vector3 RandomPointOnMask(Vector3 patrolStartPos, float patrolRange, GridMask range) {
        Vector3 randomPoint = new Vector3(UnityEngine.Random.Range(patrolStartPos.x - patrolRange, patrolStartPos.x + patrolRange)
            , UnityEngine.Random.Range(patrolStartPos.y-patrolRange, patrolStartPos.y+ patrolRange));
        return ClosestToTarget(patrolStartPos, randomPoint, range);
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
    internal static Vector3 MaxRangeOnMask(Vector3 curSlot, Vector3 target, GridMask moveMask, GridMask attackMask) {
        // gets slot where unit should be moved to still be in attack range
        curSlot = GridManager.SnapPoint(curSlot);
        target = GridManager.SnapPoint(target);
        Vector3 optimalMovePos = NbrOrMaskPos(curSlot, target, attackMask);
        Vector3 viable = ClosestToTarget(curSlot, target, moveMask);
        Vector3[] list = new Vector3[2] { optimalMovePos, viable };
        list = FilterByMask(list, curSlot, moveMask);
        if (list.Length == 2)
            list = new Vector3[1] { optimalMovePos };
        float[] dists = GetDistances(target, list);
        return list[GetIndexOfMin(dists)]; 
    }

    /// <summary>
    /// Pick a slot on the mask, and closest to target.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are applying mask</param>
    /// <returns></returns>
    public static Vector3 ClosestToTarget(Vector3 maskSource, Vector3 targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        maskSource = GridManager.SnapPoint(maskSource);
        targetSlot = GridManager.SnapPoint(targetSlot);

        Vector3 dir = (targetSlot - maskSource).normalized;
        Vector3[] maskAroundSource = mask.GetFreePositions(maskSource);
        if (maskAroundSource.Length == 0) {
            Debug.Log("Mask is zero. Aborting.");
            return maskSource;
        }
        float[] distsToTarget = GetDistances(targetSlot, maskAroundSource);
        float[] distsToSource = GetDistances(maskSource, maskAroundSource);
        // Closest slot in max range is the slot with minimum summed distance.
        // Slot that also on edge, is furthest away from mask and closest to source
        int index = IndexOfClosestToTarget(distsToTarget, distsToSource);
        return maskAroundSource[index];
    }

    public static int IndexOfClosestToTarget(float[] distsToTarget, float[] distsToSource) {
        int index = -1;
        float minSum = float.MaxValue;
        float minDistFromTarget = float.MaxValue;
        float maxDistFromSource = float.MinValue;
        for (int i = 0; i < distsToTarget.Length && i < distsToSource.Length; i++) {
            if (distsToTarget[i] + distsToSource[i] < minSum
                && distsToTarget[i] <= minDistFromTarget && distsToSource[i] >= maxDistFromSource) {
                minSum = distsToTarget[i] + distsToSource[i];
                minDistFromTarget = distsToTarget[i];
                maxDistFromSource = distsToSource[i];
                index = i;
            }
        }
        return index;
    }

    /// <summary>
    /// Gets slot with shortest distance of all slots in range, on edge of mask.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are applying mask</param>
    /// <returns></returns>
    public static Vector3 NbrOrMaskPos(Vector3 pos, Vector3 targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        pos = GridManager.SnapPoint(pos);
        targetSlot = GridManager.SnapPoint(targetSlot);
        Vector3 dir = (targetSlot - pos);
        dir.Normalize();
        Vector3[] nbrs = mask.GetFreePositions(targetSlot);
        float[] distsToTarget = GetDistances(targetSlot + dir, nbrs);
        float[] distsToSource = GetDistances(pos - dir, nbrs);
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
    public static Vector3 FurthestAwayFromTarget(Vector3 pos, Vector3 targetSlot, GridMask mask) {
        // Dir from target to source, then take closest neighbour to it.
        pos = GridManager.SnapPoint(pos);
        targetSlot = GridManager.SnapPoint(targetSlot);
        Vector3 dir = (targetSlot - pos).normalized;
        Vector3[] nbrs = mask.GetFreePositions(targetSlot);
        float[] distsToTarget = GetDistances(targetSlot - dir, nbrs);
        float[] distsToSource = GetDistances(pos + dir, nbrs);
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
    /// Gets closest slot next to target slot.
    /// </summary>
    /// <param name="pos">pos which defines which side we want</param>
    /// <param name="targetSlot">attacked unit, to which we are getting slot</param>
    /// <returns></returns>
    public static Vector3 ClosestNbour(Vector3 pos, Vector3 targetSlot) {
        // Dir from target to source, then take closest neighbour to it.
        Vector3[] nbrs = FreeNeighbours(targetSlot);
        Vector3 dir = (targetSlot - pos).normalized;
        float[] dists = GetDistances(targetSlot - dir, nbrs);
        return nbrs[dists.GetIndexOfMin()];
    }
    public static Unit ClosestUnit(Vector3 pos, Unit[] visibleUnits) {
        float[] dists = pos.GetDistances(visibleUnits);
        int closestUnitIndex = dists.GetIndexOfMin();

        return visibleUnits[closestUnitIndex];
    }
    internal static Vector3 ClosestToTargetOverMask(Vector3 source, Vector3 targetSlot, GridMask mask) {
        source = GridManager.SnapPoint(source);
        targetSlot = GridManager.SnapPoint(targetSlot);
        Vector3[] res = new Vector3[5];
        Vector3[] nbrs = FreeNeighbours(targetSlot);
        for (int i = 0; i < nbrs.Length; i++) {
            res[i] = nbrs[i];
        }
        res[4] = (ClosestToTarget(source, targetSlot, mask));
        //Debug.Log(" furthest "+ res[4] + "" +mask.IsPosInMask(source, res[4]));
        res = FilterByMask(res, source, mask);
        //Debug.Log("FILTER Lne "+res.Length);
        if (res.Length == 0) {
            Debug.Log("no moves, moving to self");
            return source;
        }
        Vector3 sourceToTarget = (targetSlot - source).normalized;
        float[] dists = GetDistances(targetSlot - sourceToTarget, res);
        return res[dists.GetIndexOfMin()];
    }


    public static Vector3[] FilterByMask(Vector3[] items, Vector3 source, GridMask mask) {
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < items.Length; i++) {
            if (GridLookup.IsPosInMask(source, items[i], mask)) {
                list.Add(items[i]);
            }
        }
        return list.ToArray();
    }

    public static bool IsNeighbour(Vector3 slot, Vector3 other) {
        other = GridManager.SnapPoint(other);
        Vector3[] slots = FreeNeighbours(slot);
        for (int i = 0; i < slots.Length; i++) {
            if (slots[i].x == other.x && slots[i].y == other.y) {
                return true;
            }
        }
        return false;
    }
    public static Vector3[] FreeNeighbours(Vector3 slot) {
        Vector3[] slots = Neighbours(slot);
        int free = slots.Length;
        bool[] taken = new bool[slots.Length];
        for (int i = 0; i < slots.Length; i++) {
            if (GridAccess.GetUnitAtPos(slots[i])) {
                taken[i] = true;
                free--;
            }
        }
        Vector3[] nslots = new Vector3[free];
        int n = 0;
        for (int i = 0; i < slots.Length; i++) {
            if (!taken[i]) {
                nslots[n] = slots[i];
                n++;
            }
        }
        return nslots;
    }
    public static Vector3[] Neighbours(Vector3 slot) {
        slot = GridManager.SnapPoint(slot);
        float x = GridManager.m.itemDimensions.x;
        float y = GridManager.m.itemDimensions.y;

        Vector3[] slots = new Vector3[4];
        slots[0] = new Vector3(slot.x - x, slot.y, slot.z);
        slots[1] = new Vector3(slot.x, slot.y - y, slot.z);
        slots[2] = new Vector3(slot.x + x, slot.y, slot.z);
        slots[3] = new Vector3(slot.x, slot.y + y, slot.z);

        return slots;
    }
}
