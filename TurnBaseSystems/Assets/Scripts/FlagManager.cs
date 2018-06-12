using System.Collections.Generic;
public class FlagManager {
    public static List<FlagController> flags = new List<FlagController>();

    public static void RegisterUnit(Unit u) {
        flags[u.flag.allianceId].units.Add(u);
    }
}