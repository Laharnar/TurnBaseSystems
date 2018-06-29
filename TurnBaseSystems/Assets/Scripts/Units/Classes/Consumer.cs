using System;
public class Consumer : UnitAbilities, IEndTurnAbilities {


    public RangedAttack basicMelle;
    public PickItem pickWeapon;
    public PassEquipped passWeapon;
    public LifeDrain deathCall;
    public FloraDrain waste;

    public RestoreAP restoration;

    Unit unit;

    private void Start() {
        unit = GetComponent<Unit>();
    }

    public override Attack BasicAttack {
        get {
            return basicMelle;
        }
    }

    
    public override GridMask BasicMask {
        get {
            return basicMelle.attackMask;
        }
    }

    public override EnvirounmentalAttack[] GetEnvAbilities() {
        throw new NotImplementedException();
    }

    public override Attack[] GetNormalAbilities() {
        return new Attack[] { basicMelle, pickWeapon, passWeapon, deathCall, waste };
    }

    public Attack[] GetPassive() {
        return new Attack[] { restoration };
    }
}
