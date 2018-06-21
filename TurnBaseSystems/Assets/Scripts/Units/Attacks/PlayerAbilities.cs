using System;
public class PlayerAbilities : UnitAbilities {
    public RangedAttack shoot1;

    public override Attack BasicAttack {
        get {
            return shoot1;
        }
    }

    public override GridMask BasicMask {
        get {
            return shoot1.attackMask;
        }
    }

    public override EnvirounmentalAttack[] GetEnvAbilities() {
        return new EnvirounmentalAttack[] { };
    }

    public override Attack[] GetNormalAbilities() {
        return new Attack[] { shoot1 };
    }
}
