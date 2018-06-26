using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base weapon, contains stats, if it's dropped etc..
/// </summary>
public class Weapon:MonoBehaviour,IInteractible {
    public static List<Weapon> weapons = new List<Weapon>();

    public float accuracy = 1;
    public int damage = 1;
    public int thrownDamage = 1;
    public bool dropped = true;
    public Transform conePref;

    public RangedAttack attackData;
    public Attack StandardAttack { get { return attackData; } }


    public int enhanceCounter = 0;
    public WeaponEnhancedEffect enhanced;

    private void Awake() {
        weapons.Add(this);
    }

    public void ApplyDamage(Unit source, GridItem attackedSlot) {
        if (UnityEngine.Random.Range(0f, 1f) <= accuracy) {
            if (attackedSlot.filledBy) {
                attackedSlot.filledBy.GetDamaged(damage);
            }
        }
    }

    public static GridItem BelongsTo(Weapon wep) {
        return GridManager.SnapToGrid(wep.transform.position);
    }

    internal void Enhance(int numOfTurns) {
        if (enhanced) {
            enhanceCounter = numOfTurns;
            enhanced.OnEquipEffect(this);
        } else Debug.Log("No enhance ability on this weapon");
    }

    public static void AssignAllDroppedWeaponsToSlots() {
        for (int i = 0; i < Weapon.weapons.Count; i++) {
            if (!Weapon.weapons[i].dropped)
                continue;
            GridItem it = Weapon.BelongsTo(Weapon.weapons[i]);
            if (it) {
                it.fillAsPickup = Weapon.weapons[i];
                if (it.fillAsPickup) {
                    it.slotInteractions.interactions.AddRange(it.fillAsPickup.GetComponent<InteractiveEnvirounment>().Copies());
                }
            }
        }
    }

    internal void OnDamageEnhanceEffect(Unit selectedPlayerUnit, GridItem attackedSlot, Unit attackedUnit, Attack curAttack) {
        if (enhanceCounter > 0) {
            enhanceCounter--;
            enhanced.OnDamageEnhanceEffect(selectedPlayerUnit, attackedSlot, attackedUnit, this);
        }
    }

}
