using System;
public class Collector : UnitAbilities {
    public RangedAttack melleAttack;
    public Hunker defensive;
    public Enhance enhanceItem;
    public PickItem pickItem;
    public PassEquipped passWeapon;
    public AoeMaskAttack blastAttack;

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

    public override Attack[] GetNormalAbilities() {
        return new Attack[] { melleAttack, defensive, enhanceItem , pickItem, passWeapon, blastAttack };
    }
}
