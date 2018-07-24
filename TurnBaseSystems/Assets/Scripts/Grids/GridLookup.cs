using UnityEngine;
public static class GridLookup {

    /// <summary>
    /// Checks if target is in mask of source.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static bool IsPosInMask(Vector2 sourcexy, Vector2 targetxy, GridMask mask, Grid grid) {
        return IsPosInMask(sourcexy, targetxy, mask);
        /*if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask width or height is 0.");
            return false;
        }

        Vector2 differenceij = mask.AsMaskCoordinates(sourcexy, targetxy);

       // int i = (target.gridX - source.gridX) + mask.w / 2;
        //int j = (target.gridY - source.gridY) + mask.l / 2;
        
        //if (i > -1 && i < mask.w && j > -1 && j < mask.l) {
        return mask.Get(differenceij);*/
        //}
        //return false;
    }

    internal static bool IsPosInMask(Vector2 sourcexy, Vector2 targetxy, GridMask mask) {
        if (mask.w == 0 || mask.l == 0) {
            Debug.LogError("Mask width or height is 0.");
            return false;
        }
        Vector2 differenceij = mask.AsMaskCoordinates(sourcexy, targetxy);
        return mask.Get(differenceij);
        /*
        Vector2 toGridSpace = GridManager.SnapPoint(position);
        int i = (target.gridX - (int)toGridSpace.x) + mask.w / 2;
        int j = (target.gridY - (int)toGridSpace.y) + mask.l / 2;
        if (i > -1 && i < mask.w && j > -1 && j < mask.l) {
            return mask.Get(differenceij);
        }
        return false;*/
    }
}
