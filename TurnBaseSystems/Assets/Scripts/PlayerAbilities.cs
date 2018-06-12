using System;
public class PlayerAbilities : UnitAbilities {
    public RangedAttack shoot1;

    public override Attack BasicAttack {
        get {
            return shoot1;
        }
    }
}
