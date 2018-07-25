using System.Collections;
using UnityEngine;
public class DebugGrid{
    public static IEnumerator BlinkColor(params Vector3[] grids) {
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.TmpDisplayGrid(grids[i], 5, GridMask.One);
            //grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.TmpHideGrid(grids[i], GridMask.One);
            //grids[i].RecolorSlot(0);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.TmpDisplayGrid(grids[i], 5, GridMask.One);
            //grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            GridDisplay.TmpHideGrid(grids[i], GridMask.One);
            //grids[i].RecolorSlot(0);
        }
    }
}
