﻿using System;
[System.Serializable]
public class BuilderClass : UnitAbilities {

    /*public RangedAttack melleAttack;
    //public TwoStepAttack deconstruct;
    //public TwoStepAttack construct;
    public SlotConsumption deconstruct;
    public SlotBuilding construct;
    public PickItem pickWeapon;
    public PassEquipped passWeapon;
    */
    public AttackData melleAttack;
    //public TwoStepAttack deconstruct;
    //public TwoStepAttack construct;
    public AttackData deconstruct;
    public AttackData construct;
    public AttackData pickWeapon;
    public AttackData passWeapon;

    public override AttackData[] GetNormalAbilities() {
        return AddAbilities(new AttackData[] { melleAttack, deconstruct, construct, pickWeapon, passWeapon });
    }
}
