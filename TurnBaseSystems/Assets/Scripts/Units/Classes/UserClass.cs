using UnityEngine;

public class UserClass : UnitAbilities {
    /*
    public RangedAttack shoot1;
    public ThrowEquipped throwWeapon;
    public AttackWithEquipped melleWeaponAttack;
    public PickItem pickWeapon;
    public PassEquipped passWeapon;
    */
    public AttackData shoot1;
    public AttackData throwWeapon;
    public AttackData melleWeaponAttack;
    public AttackData pickWeapon;
    public AttackData passWeapon;

    Unit unit;

    private void Start() {
        unit = GetComponent<Unit>();
    }

    public override AttackData[] GetNormalAbilities() {
        return AddAbilities(new AttackData[] { BasicAttack, throwWeapon, melleWeaponAttack, pickWeapon, passWeapon });
    }

    public override AttackData BasicAttack {
        get {
            return unit.equippedWeapon!= null ? unit.equippedWeapon.StandardAttack as AttackData : shoot1 as AttackData;
        }
    }

    public override GridMask BasicMask {
        get {
            return unit.equippedWeapon != null ? unit.equippedWeapon.StandardAttack.attackMask : shoot1.attackMask;
        }
    }
}
