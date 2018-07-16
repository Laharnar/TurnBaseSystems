using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitAbilities : MonoBehaviour {
    public AttackData move;
    public GridMask BasicMask { get; }
    public AttackData MoveAction { get { return move; } }

    public List<AttackData> additionalAbilities = new List<AttackData>();


    public virtual AttackData[] GetNormalAbilities() {
        List<AttackData> data = new AttackData[] { move }.ToList();
        data.AddRange(additionalAbilities);
        return data.ToArray();
    }

    protected AttackData[] AddAbilities(AttackData[] data) {
        
        List<AttackData> d = new List<AttackData>();
        
        d.Add(move);
        d.AddRange(additionalAbilities);
        d.AddRange(data);
        return d.ToArray();
    }

    public void SaveNewAbilities(AttackData[] ndata) {

        AttackData[] odata = GetNormalAbilities();

        for (int i = 0; i < odata.Length; i++) {
            odata[i].actionCost = ndata[i].actionCost;
            odata[i].animData.animLength = ndata[i].animData.animLength;
            odata[i].animData.animTrigger = ndata[i].animData.animTrigger;
            odata[i].animData.useInfo = ndata[i].animData.useInfo;

            odata[i].attackFunction = ndata[i].attackFunction;

            odata[i].attackMask = ndata[i].attackMask;
            odata[i].attackType = ndata[i].attackType;
            odata[i].attackType_EditorOnly = ndata[i].attackType_EditorOnly;
            odata[i].requiresUnit = ndata[i].requiresUnit;
            odata[i].o_attackName = ndata[i].o_attackName;
        }
    }
}

