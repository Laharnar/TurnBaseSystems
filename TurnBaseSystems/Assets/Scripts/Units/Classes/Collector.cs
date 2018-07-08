using System;
public class Collector : UnitAbilities {
/*    public RangedAttack melleAttack;
    public Hunker defensive;
    public Enhance enhanceItem;
    public PickItem pickItem;
    public PassEquipped passWeapon;
    public AoeMaskAttack blastAttack;
    */
    public AttackData melleAttack;
    public AttackData defensive;
    public AttackData enhanceItem;
    public AttackData pickItem;
    public AttackData passWeapon;
    public AttackData blastAttack;


    public override AttackData BasicAttack {
        get {
            return melleAttack;
        }
    }

    public override GridMask BasicMask {
        get {
            return melleAttack.attackMask;
        }
    }

    public override AttackData[] GetNormalAbilities() {
        return new AttackData[] { melleAttack, defensive, enhanceItem , pickItem, passWeapon, blastAttack };
    }
}
