using System;
using System.Collections.Generic;
public class FlagManager {
    public static List<FlagController> flags = new List<FlagController>();
    
    public static void RegisterUnit(Unit u) {
        if (flags.Count < u.flag.allianceId) {
            CombatManager.m.Init();
        }

        flags[u.flag.allianceId].units.Add(u);
    }

    internal static void DeRegisterUnit(Unit u) {
        flags[u.flag.allianceId].units.Remove(u);
    }

}