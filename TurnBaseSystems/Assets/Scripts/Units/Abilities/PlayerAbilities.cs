using System;
public class PlayerAbilities : UnitAbilities {
    //public RangedAttack shoot1;
    public AttackData shoot1;

    public override AttackData BasicAttack {
        get {
            return shoot1;
        }
    }

    public override GridMask BasicMask {
        get {
            return shoot1.attackMask;
        }
    }

    public override AttackData[] GetNormalAbilities() {
        return AddAbilities(new AttackData[] { shoot1 });
    }
}
