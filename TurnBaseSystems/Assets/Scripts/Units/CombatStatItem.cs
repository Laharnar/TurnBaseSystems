using UnityEngine;
public class CombatStatItem {
    public CombatStatType statType;
    public int value;
    public AbilityEffect dataSource;
    /// <summary>
    /// Consumes as much of value as possible.
    /// Returns true if this item is completly emptied.
    /// False if it's not.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool Consume(CombatStatType statType, ref int value) {
        if (this.statType != statType) {
            return false;
        }
        int v = this.value;
        this.value -= value;
        value = Mathf.Clamp(value - v, 0, value);
        if (this.value <= 0) {
            return true;
        }
        return false;
    }
}
