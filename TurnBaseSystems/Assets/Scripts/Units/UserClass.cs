using System;
[System.Serializable]
public class BuilderClass : UnitAbilities {

    public RangedAttack melleAttack;
    //public TwoStepAttack deconstruct;
    //public TwoStepAttack construct;
    public SlotConsumption deconstruct;
    public SlotBuilding construct;

    public override Attack BasicAttack {
        get {
            return melleAttack;
        }
    }

    public override GridMask BasicMask {
        get {
            return melleAttack.attackMask;
        }
    }

    public override EnvirounmentalAttack[] GetEnvAbilities() {
        throw new NotImplementedException();
    }

    public override Attack[] GetNormalAbilities() {
        return new Attack[] { melleAttack };
    }
}
[System.Serializable]
public class UserClass : UnitAbilities {

    public RangedAttack shoot1;
    public ThrowEquipped throwWeapon;
    public AttackWithEquipped melleWeaponAttack;
    public PickItem pickWeapon;

    Unit unit;

    private void Start() {
        unit = GetComponent<Unit>();
    }

    public override EnvirounmentalAttack[] GetEnvAbilities() {
        return new EnvirounmentalAttack[] { pickWeapon };
    }

    public override Attack[] GetNormalAbilities() {
        return new Attack[] { BasicAttack, throwWeapon, melleWeaponAttack };
    }

    public override Attack BasicAttack {
        get {
            return unit.equippedWeapon!= null ? unit.equippedWeapon.StandardAttack as Attack : shoot1 as Attack;
        }
    }

    public override GridMask BasicMask {
        get {
            return unit.equippedWeapon != null ? unit.equippedWeapon.StandardAttack.attackMask : shoot1.attackMask;
        }
    }
}
