using System;
using System.Collections.Generic;
public class FlagInfo {
    public List<Unit> units = new List<Unit>();
}
public class FlagManager {
    public FlagInfo info = new FlagInfo();
    public FlagController controller;
    public int id;

    public FlagManager(FlagController enemyFlag, int id) {
        info = new FlagInfo();
        this.controller = enemyFlag;
        this.id = id;
    }

    public void NullifyUnits() {
        List<Unit> units = info.units;
        for (int i = 0; i < units.Count; i++) {
            if (units[i] == null) {
                units.RemoveAt(i);
                i--;
            }
        }
    }
}