using System;
using System.Collections.Generic;
using System.Linq;

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

    /*public override StdAttackData[] GetNormalAbilities() {
        return AddAbilities(new StdAttackData[]{ basicMelle, pickWeapon, passWeapon, deathCall, waste });
    }*/
    
    public AttackData[] GetPassive() {
        return new AttackData[] { restoration };
    }
}
