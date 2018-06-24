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
        return new Attack[] { melleAttack, deconstruct, construct };
    }
}
