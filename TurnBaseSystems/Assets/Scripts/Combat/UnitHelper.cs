using System.Collections.Generic;
public static class UnitHelper {
    public static List<Unit> GetVisibleUnits(this List<Unit> search, Unit[] ignored) {
        List<Unit> f = new List<Unit>();
        for (int i = 0; i < search.Count; i++) {
            bool onIgnoreList = false;
            for (int j = 0; j < ignored.Length; j++) {
                if (ignored[j] == search[i]) {
                    onIgnoreList = true;
                    break;
                }
            }
            if (!onIgnoreList && search[i].combatStatus != CombatStatus.Invisible) {
                f.Add(search[i]);
            }
        }
        return f;
    }
}
