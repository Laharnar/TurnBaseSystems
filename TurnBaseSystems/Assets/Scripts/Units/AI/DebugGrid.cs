using System.Collections;
using UnityEngine;
public class DebugGrid{
    public static IEnumerator BlinkColor(params Vector3[] grids) {
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.Instance.SetUpGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < grids.Length; i++) {
            //GridDisplay.TmpHideGrid(0, grids[i], GridMask.One);
            GridDisplay.Instance.HideGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.Instance.SetUpGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
        yield return new WaitForSeconds(0.15f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.Instance.HideGrid(grids[i], GridDisplayLayer.AIAction, GridMask.One);
        }
    }
}
