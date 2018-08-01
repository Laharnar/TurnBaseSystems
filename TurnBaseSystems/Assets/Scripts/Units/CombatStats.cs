using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores stats. Stats can be summed. It's like a list of current buffs.
/// </summary>
[System.Serializable]
public class CombatStats {
    public List<CombatStatItem> powers = new List<CombatStatItem>();

    void Increase(CombatStatType type, int amount) {
        powers.Add(new CombatStatItem() {  statType = type, value = amount });
    }
    /// <summary>
    /// Reduce only dmg by all types.
    /// Use when applying damage first time.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void Reduce(CombatStatType type, int amount) {
        Debug.Log("amount "+amount);
        for (int i = 0; i < powers.Count; i++) {
            if (powers[i].Consume(type, ref amount)) {
                Debug.Log("Power consumed");
                powers.RemoveAt(i);
                i--;
            }
            Debug.Log("amount "+i+" "+amount);
            if (amount == 0) {
                break;
            }
        }
    }
    /// <summary>
    /// Reduce only dmg by specific type.
    /// Use for callback when buffs have to be removed, like with aura.
    /// Removing armor while in aura works fine with this.
    /// </summary>
    /// <param name="empowerAlliesData"></param>
    /// <param name="type"></param>
    /// <param name="amount"></param>
    public void Reduce(AttackDataType buffSource, CombatStatType type, int amount) {
        for (int i = 0; i < powers.Count; i++) {
            if (powers[i].dataSource == buffSource) {
                if (powers[i].Consume(type, ref amount)) {
                    powers.RemoveAt(i);
                    i--;
                }
                if (amount == 0) {
                    break;
                }
            }
        }
    }

    internal void Increase(AttackDataType empowerAlliesData, CombatStatType type, int amount) {
        powers.Add(new CombatStatItem() { dataSource=empowerAlliesData,  statType = type, value = amount });
    }

    public int GetSum(CombatStatType type) {
        int r = 0;
        for (int i = 0; i < powers.Count; i++) {
            if (powers[i].statType == type) {
                r += powers[i].value;
            }
        }
        return r;
    }
    internal int GetSum(AttackDataType d1, CombatStatType type) {
        int r = 0;
        for (int i = 0; i < powers.Count; i++) {
            if (powers[i].statType == type && powers[i].dataSource == d1) {
                r += powers[i].value;
            }
        }
        return r;
    }
    internal int GetSum(AttackDataType d1, int repetitiveIndex, CombatStatType type) {
        int r = 0;
        int repeat = 0;
        for (int i = 0; i < powers.Count; i++) {
            if (powers[i].statType == type && powers[i].dataSource == d1) {
                if (repetitiveIndex == repeat)
                    r += powers[i].value;
                repeat++;
            }
        }
        return r;
    }

    /*internal void Set(CombatStatType type, int nvalue) {
        int v = GetSum(type);
        if (v < nvalue) { // new value is bigger
            Increase(type, nvalue-v);
        } else if (v > nvalue) {
            Reduce(type, v-nvalue);
        }
    }*/

    /// <summary>
    /// Adds or reduces to new value.
    /// Warning: apply original source, so the copies can be found from it.
    /// </summary>
    /// <param name="abilitySource"></param>
    /// <param name="type"></param>
    /// <param name="nvalue"></param>
    internal void Set(AttackDataType originalSource, CombatStatType type, int nvalue) {
        int v = GetSum(type);
        if (v < nvalue) { // new value is bigger
            Increase(originalSource, type, nvalue - v);
        } else if (v > nvalue) {
            //Debug.Log("Reducing " +v + " "+nvalue + " to "+(v - nvalue));
            Reduce(originalSource, type, v - nvalue);
        }
    }
}
