using System;
using System.Collections.Generic;

public class InteractionScanner {
    /// <summary>
    /// Finds all slots that unit can interact with.
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

    /// <summary>
    /// Adds to list without duplicate slots.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="items"></param>
    /// <returns></returns>
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