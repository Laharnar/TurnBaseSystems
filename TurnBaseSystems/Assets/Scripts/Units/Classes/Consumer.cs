using System;
public class Consumer : UnitAbilities, IEndTurnAbilities {

    /*
    public RangedAttack basicMelle;
    public PickItem pickWeapon;
    public PassEquipped passWeapon;
    public LifeDrain deathCall;
    public FloraDrain waste;

    public RestoreAP restoration;
    */

    public AttackData basicMelle;
    public AttackData pickWeapon;
    public AttackData passWeapon;
    public AttackData deathCall;
    public AttackData waste;
    public AttackData restoration;


    Unit unit;

    private void Start() {
        unit = GetComponent<Unit>();
    }

    public override AttackData BasicAttack {
        get {
            return basicMelle;
        }
    }

    
    public override GridMask BasicMask {
        get {
            return basicMelle.attackMask;
        }
    }

    public override AttackData[] GetNormalAbilities() {
        return new AttackData[] { basicMelle, pickWeapon, passWeapon, deathCall, waste };
    }

    public AttackData[] GetPassive() {
        return new AttackData[] { restoration };
    }
}
