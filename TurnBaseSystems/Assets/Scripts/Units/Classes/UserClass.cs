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

    /*public override StdAttackData[] GetNormalAbilities() {
        return AddAbilities(new StdAttackData[] { throwWeapon, melleWeaponAttack, pickWeapon, passWeapon });
    }*/
}
