using System;
using System.Collections.Generic;

public class InteractionScanner {
    /// <summary>
    /// So what this should do, is take all of player's env abilities, and check slots
    /// around of something is in range.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static GridItem[] Scan(Unit unit) {
        List<GridItem> scanned = new List<GridItem>();

        EnvirounmentalAttack[] envAttacks = unit.abilities.GetEnvAbilities();
        for (int i = 0; i < envAttacks.Length; i++) {
            AddRangeUnique(scanned, GridManager.GetSlotsInInteractiveRange(unit, envAttacks[i].attackMask));
        }

        return scanned.ToArray();//GridManager.GetSlotsInInteractiveRange(unit, null);
    }

    public static List<GridItem> AddRangeUnique(List<GridItem> list, params GridItem[] items) {
        for (int i = 0; i < items.Length; i++) {
            if (list.Contains(items[i])) {
                continue;
            }
            list.Add(items[i]);
        }
        return list;
    }
}