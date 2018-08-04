public class DamageInfo {
    public int dmg;
    public DamageType dmgType;
    public EnergyType energy;
    public DamageAttribute atributte;

    public DamageInfo(int v) {
        this.dmg = v;
        dmgType = DamageType.Psyhical;
        energy = EnergyType.None;
        atributte = DamageAttribute.HardObject;
    }

    public DamageInfo(int dmg, DamageType dmgType, EnergyType energy, DamageAttribute atrib) {
        this.dmg = dmg;
        this.dmgType = dmgType;
        this.energy = energy;
        this.atributte = atrib;
    }
}
