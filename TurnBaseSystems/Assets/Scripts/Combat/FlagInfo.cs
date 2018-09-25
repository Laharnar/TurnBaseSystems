using System.Collections.Generic;
using UnityEngine;
public class FlagInfo {
    public List<Unit> units = new List<Unit>();

    public static explicit operator Transform[](FlagInfo info) {
        Transform[] t = new Transform[info.units.Count];
        for (int i = 0; i < t.Length; i++) {
            t[i] = info.units[i].transform;
        }
        return t;
    }

    public void NullifyUnits() {
        for (int i = 0; i < units.Count; i++) {
            if (units[i] == null) {
                units.RemoveAt(i);
                i--;
            }
        }
    }
}
