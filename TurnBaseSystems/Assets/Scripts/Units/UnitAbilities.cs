using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class UnitAbilities : MonoBehaviour {
    public bool newVersion = false;
    public AttackData move;
    public AttackData2 move2;

    public List<AttackData> additionalAbilities = new List<AttackData>();
    public List<AttackData2> additionalAbilities2 = new List<AttackData2>();

    public AnimDataHolder abilityAnimations;

    private void Awake() {
        if (abilityAnimations == null) {
            abilityAnimations = GetComponent<AnimDataHolder>();
        }
    }

    public virtual AttackData[] GetNormalAbilities() {
        List<StdAttackData> data = new StdAttackData[] { move }.ToList();
        if (newVersion)
            data.AddRange(additionalAbilities2.ToArray());
        else data.AddRange(additionalAbilities.ToArray());
        return data.ToArray() as AttackData[];
    }

    protected StdAttackData[] AddAbilities(StdAttackData[] data) {
        
        List<StdAttackData> d = new List<StdAttackData>();
        
        d.Add(move);
        if (newVersion)
            d.AddRange(additionalAbilities2.ToArray());
        else d.AddRange(additionalAbilities.ToArray());
        d.AddRange(data);
        return d.ToArray();
    }

    public void SaveNewAbilities(AttackData[] ndata) {

        AttackData[] odata = GetNormalAbilities() as AttackData[];

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

    public static AttackBaseType GetAttackType(AttackType atkType) {
        switch (atkType) {
            case AttackType.SingleTarget:
                return new RangedAttack();
            case AttackType.Aura:
                return new AoeMaskAttack();
            case AttackType.LongRangeAoe:// note: aoe mask attack is for cone attacks, not mouse
                return new AoeMaskAttack();
            case AttackType.Hunker:
                return new Hunker();
            case AttackType.Pickup:
                return new PickItem();
            case AttackType.ThrowEquipped:
                return new ThrowEquipped();
            case AttackType.EquippedWeapon:
                return new AttackWithEquipped();
            case AttackType.EnhanceWeapon:
                return new Enhance();
            case AttackType.Building:
                return new SlotBuilding();
            case AttackType.Deconstruct:
                return new SlotConsumption();
            case AttackType.GroundDrain:
                return new FloraDrain();
            case AttackType.UnitDrain:
                return new LifeDrain();
            case AttackType.RestoreAP:
                return new RestoreAP();
            default:
                break;
        }
        return new RangedAttack();
    }

}

