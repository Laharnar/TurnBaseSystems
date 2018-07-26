using System;
using UnityEngine;
public static class GridLookup {

    internal static bool IsPosInMask(Vector2 sourcexy, Vector2 targetxy, GridMask mask) {
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask width or height is 0.");
            return false;
        }
        return mask.IsPosInMask(sourcexy, targetxy);
    }

}
