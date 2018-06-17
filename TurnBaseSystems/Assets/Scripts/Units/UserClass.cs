[System.Serializable]
public class UserClass : UnitAbilities {

    public RangedAttack shoot1;
    public ThrowEquipped throwWeapon;
    public AttackEquipped melleWeaponAttack;

    Unit unit;

    private void Start() {
        unit = GetComponent<Unit>();
    }

    public override Attack BasicAttack {
        get {
            return unit.equippedWeapon!= null ? melleWeaponAttack as Attack : shoot1 as Attack;
        }
    }
}