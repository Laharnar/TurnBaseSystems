using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlagController {
    public List<Unit> units = new List<Unit>();
    public bool turnDone = false;

    public bool DetectedSomeone { get {
            for (int i = 0; i < units.Count; i++) {
                if (units[i].detection.detectedSomeone) {
                    return true;
                }
            }
            return false;
        } }

    public abstract IEnumerator FlagUpdate();

    public void NullifyUnits() {
        for (int i = 0; i < units.Count; i++) {
            if (units[i] == null) {
                units.RemoveAt(i);
                i--;
            }
        }
    }

}