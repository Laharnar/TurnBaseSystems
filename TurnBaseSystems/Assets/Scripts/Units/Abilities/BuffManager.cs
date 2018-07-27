using System;
using System.Collections.Generic;
using UnityEngine;
public class BuffUnitData {
    public BUFFAttackData buff;
    public Unit source;
}
public class CopyReferences {
    public static Dictionary<BUFFAttackData, List<BuffUnitData>> sourceCopies = new Dictionary<BUFFAttackData, List<BuffUnitData>>();

    internal static void Add(Unit source, BUFFAttackData attackDataType, BUFFAttackData d) {
        if (!sourceCopies.ContainsKey( attackDataType)) {
            sourceCopies.Add(attackDataType, new List<BuffUnitData>());
        }

        sourceCopies[attackDataType].Add(new BuffUnitData() { buff = d, source=source  });
    }
}
public class BuffManager {

    //public static List<BUFFAttackData> registeredBuffs = new List<BUFFAttackData>();
    //public static List<Unit> registeredBuffSources = new List<Unit>();

    public static void Register(Unit source, BUFFAttackData attackDataType) {
        //registeredBuffSources.Add(source);
        BUFFAttackData d = attackDataType.Copy();
        //registeredBuffs.Add(attackDataType);

        CopyReferences.Add(source, attackDataType, d);
    }

    public static void ConsumeBuffs(int faction) {
        foreach (var item in CopyReferences.sourceCopies) {
            BUFFAttackData origData = item.Key;
            List<BuffUnitData> buffInstances = item.Value;

            for (int i = 0; i < buffInstances.Count; i++) {
                // One buff ref still stays in.
                if (buffInstances[i].source == null) {
                    buffInstances.RemoveAt(i);
                    i--;
                    continue;
                }

                // type: consume only on enemy faction.
                if (buffInstances[i].source.flag.allianceId != faction) {
                    buffInstances[i].buff.turns--;

                    // activate buff
                    BUFFAttackData buff = buffInstances[i].buff;
                    if (buff.turns <= 0) {
                        Debug.Log("Ending buff " + buff.GetType() + " " + buff.setStatus + " " + buffInstances[i].source);
                        AttackData2.RunAnimations(buffInstances[i].source, buff.endAnimSets);
                        if (buff.buffType == BuffType.Shielded) {
                            Debug.Log("[shield buff] -shield " + buff.armorAmt);
                            buffInstances[i].source.AddShield(origData, -buff.armorAmt);
                        }
                        buffInstances[i].source.combatStatus = buff.endBuffStatus;
                    }
                }
            }
        }
    }
}