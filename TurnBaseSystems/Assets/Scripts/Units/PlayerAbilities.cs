using System;
public class PlayerAbilities : UnitAbilities {
    public RangedAttack shoot1;

    public override Attack BasicAttack {
        get {
            return shoot1;
        }
    }

    public override EnvirounmentalAttack[] GetEnvAbilities() {
        return new EnvirounmentalAttack[] { };
    }
}
