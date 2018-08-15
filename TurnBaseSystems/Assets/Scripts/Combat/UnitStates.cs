using System.Collections.Generic;
public static class UnitStates {
    public static bool DetectedSomeone(List<Unit> units) {
        for (int i = 0; i < units.Count; i++) {
            if (units[i].detection.detectedSomeone) {
                return true;
            }
        }
        return false;
    }


    public static Unit[] GetVisibleUnits(List<Unit> units) {
        List<Unit> units1 = new List<Unit>();
        for (int i = 0; i < units.Count; i++) {
            if (units[i].combatStatus == CombatStatus.Normal)
                units1.Add(units[i]);
        }
        return units1.ToArray();
    }
}