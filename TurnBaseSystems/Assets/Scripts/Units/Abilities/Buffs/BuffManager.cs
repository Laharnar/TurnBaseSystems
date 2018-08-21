using System;
using System.Collections.Generic;
using UnityEngine;
public class BuffUnitData {
    public BUFFAttackData buff;
    public Unit source;
    public Unit target;
}
public class CopyReferences {
    public static Dictionary<BUFFAttackData, List<BuffUnitData>> sourceCopies = new Dictionary<BUFFAttackData, List<BuffUnitData>>();

    internal static void Add(Unit source, Unit target, BUFFAttackData AbilityEffect, BUFFAttackData d) {
        if (!sourceCopies.ContainsKey( AbilityEffect)) {
            sourceCopies.Add(AbilityEffect, new List<BuffUnitData>());
        }

        sourceCopies[AbilityEffect].Add(new BuffUnitData() { buff = d, source=source, target=target  });
    }
}
public class BuffManager {

    //public static List<BUFFAttackData> registeredBuffs = new List<BUFFAttackData>();
    //public static List<Unit> registeredBuffSources = new List<Unit>();

    public static void Register(Unit source, Unit target, BUFFAttackData AbilityEffect) {
        //registeredBuffSources.Add(source);
        BUFFAttackData d = AbilityEffect.Copy();
        //registeredBuffs.Add(AbilityEffect);

        CopyReferences.Add(source, target, AbilityEffect, d);
    }

    internal static List<BuffUnitData> GetBuffs(BUFFAttackData bUFFAttackData) {
        return CopyReferences.sourceCopies[bUFFAttackData];
    }

    public static void ConsumeBuffs(FlagManager faction) {
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
                if (buffInstances[i].source.flag.allianceId != faction.id) {
                    AbilityInfo.ActiveOrigBuff = origData;
                    AbilityInfo.ActiveBuffData = buffInstances[i];
                    AbilityInfo.Instance.executingUnit = AbilityInfo.ActiveBuffData.target;
                    buffInstances[i].buff.AtkBehaviourExecute(AbilityInfo.Instance);
                }
            }
        }
    }

    internal static void Remove(BUFFAttackData origBuff, BUFFAttackData buffInstanceOnSomeUnit, BuffUnitData dataAboutBuffInstance) {
        CopyReferences.sourceCopies[origBuff].Remove(dataAboutBuffInstance);
    }
}