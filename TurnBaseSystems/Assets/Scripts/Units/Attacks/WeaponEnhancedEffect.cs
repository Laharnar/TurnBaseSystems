using UnityEngine;
public abstract class WeaponEnhancedEffect : MonoBehaviour {
    public abstract void OnDamageEnhanceEffect(Unit unit, GridItem attackedSlot, Unit attackedUnit, Weapon weap);// supports additional damage, poison, etc.

    // to add remove auras or buffs.
    public abstract void OnEquipEffect( Weapon weap); 
    public abstract void OnDeEquipEffect( Weapon weap);
}