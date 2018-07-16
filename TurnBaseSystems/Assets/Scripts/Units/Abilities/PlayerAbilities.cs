using System;
public class PlayerAbilities : UnitAbilities {
    //public RangedAttack shoot1;
    public AttackData shoot1;


    public override AttackData[] GetNormalAbilities() {
        return AddAbilities(new AttackData[] { shoot1 });
    }
}
