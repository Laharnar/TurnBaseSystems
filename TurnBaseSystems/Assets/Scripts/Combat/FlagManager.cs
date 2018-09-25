using System;
using System.Collections.Generic;
public class FlagManager {
    public List<Flag> flags;
    bool updatedSearch = false;
    List<Unit> units;

    public List<Unit> Units {
        get {
            if (updatedSearch && units!=null)
                return units;

            updatedSearch = true;
            List<Unit> u = new List<Unit>();
            for (int i = 0; i < flags.Count; i++) {
                flags[i].NullifyUnits();
                if (flags[i] != null && flags[i].info != null)
                    u.AddRange(flags[i].info.units);
            }
            units = u;
            return u;
        }
    }

    internal void UnitNullCheck() {
        for (int j = 0; j < flags.Count; j++) {
            for (int i = 0; i < flags[j].info.units.Count; i++) {
                if (flags[j].info.units[i] == null || flags[j].info.units[i].transform == null) {
                    flags[j].info.units.RemoveAt(i);
                    i--;
                    updatedSearch = false;
                }
            }
        }
    }

    internal void AddUnit(Unit u, int allianceId) {
        flags[u.flag.allianceId].info.units.Add(u);
        updatedSearch = false;
    }

    internal static void InitInstance() {
        GameManager.Instance.RegisterManager(new FlagManager(), 0);
    }

    internal List<Unit> GetVisibleUnits(int allianceId, params Unit[] ignored) {
        return UnitHelper.GetVisibleUnits(flags[allianceId].info.units, ignored);
    }
    
}
