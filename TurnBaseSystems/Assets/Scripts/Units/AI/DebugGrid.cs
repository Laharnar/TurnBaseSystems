using System.Collections;
using UnityEngine;
public class DebugGrid{
    public static IEnumerator BlinkColor(params GridItem[] grids) {
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(0);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(0);
        }

    }
}
