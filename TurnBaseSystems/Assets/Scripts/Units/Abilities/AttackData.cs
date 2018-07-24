using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class StdAttackData {
}
/// <summary>
/// Attacks are manually coded into UnitAbilities-derived classes.
/// These are what is avaliable to units to execute.
/// Targets are units.
/// </summary>
[System.Serializable]
[System.Obsolete("Not actively used")]
public sealed class AttackData : StdAttackData {

    public string o_attackName;
    public GridMask attackMask;
    [System.Obsolete("Don't use")]
    public string attackType = "Normal";
    public int actionCost = 1;
    public bool requiresUnit = true;
    public AttackAnimationInfo animData;

    /// <summary>
    /// To change attack function by editor
    /// </summary>
    public AttackType attackType_EditorOnly;

    public AttackBaseType attackFunction;
    public AttackTypeData attackData;

    [System.Obsolete("Not actively used")]
    public void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (attackFunction == null) {
            attackFunction = AttackBaseType.GetAttackType(attackType_EditorOnly);
            Debug.Log("Auto assigning attack "+o_attackName+"as "+attackType_EditorOnly.ToString() + " "+source, source);
        }
        if (attackFunction != null)
            ApplyDamage(source, attackedSlot, this);
        //attackFunction.ApplyDamage(source, attackedSlot, attackData);
        else Debug.Log("No attack function defined");
    }

    [System.Obsolete("Not actively used")]
    public static void ApplyDamage(Unit source, GridItem attackedSlot, AttackData attack) {
        if (attackedSlot!=null) {
            Debug.Log("Missing SLOT reference. Error in getting slots.");
            return;
        }
        AttackTypeData attackData = attack.attackData;
        switch (attack.attackType_EditorOnly) {
            case AttackType.SingleTarget:
                if (attackedSlot.filledBy) {
                    attackedSlot.filledBy.GetDamaged(attackData.damage);
                }
                break;
            case AttackType.Aura:
                /*GridItem[] attackArea;
                attackArea = GridAccess.LoadLocalAoeAttackLayer(attackedSlot, attackData.aoeMask, PlayerFlag.m.mouseDirection);

                for (int i = 0; i < attackArea.Length; i++) {
                    if (attackArea[i].filledBy)
                        attackArea[i].filledBy.GetDamaged(attack.attackData.damage);
                }*/
                break;
            case AttackType.LongRangeAoe:// note: aoe mask attack is for cone attacks, not mouse
                /*attackArea = GridAccess.LoadLocalAoeAttackLayer(attackedSlot, attackData.aoeMask, PlayerFlag.m.mouseDirection);

                for (int i = 0; i < attackArea.Length; i++) {
                    if (attackArea[i].filledBy)
                        attackArea[i].filledBy.GetDamaged(attackData.damage);
                }*/
                break;
            case AttackType.Hunker:
                source.AddShield(attackData.armorAmount);
                source.RestoreAP(attackData.restoresAp);
                break;
            case AttackType.Pickup:
                /*if (attackedSlot.fillAsPickup) {
                    source.EquipAction(attackedSlot.fillAsPickup);
                    attackedSlot.DetachPickupFromSlot();
                }*/
                break;
            /*case AttackType.ThrowEquipped:
                if (source.equippedWeapon) {
                    // throw at enemy = damage, at ally = pass, at ground = de equip
                    if (attackedSlot.filledBy && source.flag != attackedSlot.filledBy.flag) {
                        attackedSlot.filledBy.GetDamaged(source.equippedWeapon.thrownDamage);
                        source.equippedWeapon.transform.position = attackedSlot.filledBy.transform.position;
                        source.DeEquip();
                    } else if (attackedSlot.filledBy && source.flag == attackedSlot.filledBy.flag) {
                        source.PassWeapon(source.equippedWeapon, attackedSlot.filledBy);
                    } else if (!attackedSlot.filledBy) {
                        source.equippedWeapon.transform.position = attackedSlot.filledBy.transform.position;
                        source.DeEquip();
                    }
                }
                source.equippedWeapon = null;
                break;
            case AttackType.EquippedWeapon:
                if (source.equippedWeapon != null && attackedSlot.filledBy) {
                    attackedSlot.filledBy.GetDamaged(source.equippedWeapon.damage);
                }
                break;
            case AttackType.EnhanceWeapon:
                if (source.equippedWeapon)
                    source.equippedWeapon.Enhance(attackData.numOfTurns);
                break;*/
            case AttackType.Building:
                if (!attackedSlot.fillAsStructure) {
                    source.materials = 0;
                    BuildingManager.m.CreateWall(attackedSlot);
                }
                break;
            case AttackType.Deconstruct:
                /*if (attackedSlot.fillAsStructure) {
                    source.materials += attackedSlot.fillAsStructure.materialValue;
                    attackedSlot.RemoveInteractions(attackedSlot.fillAsStructure.GetComponent<InteractiveEnvirounment>().interactions);
                    attackedSlot.fillAsStructure.Destruct();
                }*/
                break;
            case AttackType.GroundDrain:
                /*attackArea = GridAccess.LoadLocalAoeAttackLayer(attackedSlot, attackData.aoeMask, PlayerFlag.m.mouseDirection);

                int groundHits = 0;
                for (int i = 0; i < attackArea.Length; i++) {
                    if (!attackArea[i].filledBy) {
                        if (attackArea[i].TryDrainGround())
                            groundHits++;
                    }
                }
                source.RestoreAP(groundHits * attackData.restoreAPPerSlotHit);*/
                break;
            case AttackType.UnitDrain:
                /*attackArea = GridAccess.LoadLocalAoeAttackLayer(attackedSlot, attackData.aoeMask, PlayerFlag.m.mouseDirection);

                int unitsHit = 0;
                for (int i = 0; i < attackArea.Length; i++) {
                    if (attackArea[i].filledBy) {
                        attackArea[i].filledBy.GetDamaged(attackData.damage);
                        unitsHit++;
                    }
                }
                source.RestoreAP(unitsHit * attackData.restoreAPPerUnitHit);*/
                break;
            case AttackType.RestoreAP:
                source.RestoreAP(attackData.restoresAp);
                break;
            default:
                Debug.Log("Unhandled attack " + attack.attackType_EditorOnly);
                break;
        }
    }
}
public enum AttackType {
    SingleTarget,
    Aura,
    LongRangeAoe,
    SelfTarget,
    Pickup,
    Building,
    GroundDrain,
    UnitDrain,
    Hunker,
    EquippedWeapon,
    EnhanceWeapon,
    PassEquipped,
    ThrowEquipped,
    Deconstruct,
    RestoreAP
}

/// <summary>
/// Target is interactible envirounment.
/// Will automatically scan for interactive items in range.
/// <seealso cref="InteractiveEnvirounment"/>
/// </summary>
[System.Serializable]
public abstract class EnvirounmentalAttack:AttackBaseType {

}