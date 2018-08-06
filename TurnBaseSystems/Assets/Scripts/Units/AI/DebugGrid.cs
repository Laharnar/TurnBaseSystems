using System.Collections;
using UnityEngine;
public class DebugGrid{
    public static IEnumerator BlinkColor(params Vector3[] grids) {
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.SetUpGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            //GridDisplay.TmpHideGrid(0, grids[i], GridMask.One);
            GridDisplay.HideGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.SetUpGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.HideGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
    }
}
