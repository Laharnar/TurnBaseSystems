[System.Serializable]
public class Resistances {

    public float resistanceToPhysical = 0;
    public float resistanceToMagical = 0;
    public float resistanceToInner = 0;
    public float resistanceToOuter = 0;
    public float resistanceToHardObjects = 0;
    public float resistanceToCombustion = 0;
    public float resistanceToSolidification = 0;
    public float resistanceToShockwaves = 0;

    public float ApplyResistance(float dmg, DamageInfo dmgInfo) {
        float[] resistancesEnergy = new float[] {
            0,
            resistanceToOuter,
            resistanceToInner
        };
        float[] resistancesAtrib = new float[] {
            resistanceToHardObjects,
            resistanceToCombustion,
            resistanceToShockwaves,
            resistanceToSolidification
        };
        float[] resistancesDmgType = new float[] {
            resistanceToPhysical,
            resistanceToMagical,
        };

        float f = (dmg * (1f-resistancesAtrib[(int)dmgInfo.atributte]));
        dmg = ((dmg) * (1f - resistancesAtrib[(int)dmgInfo.atributte]));
        dmg = ((dmg) * (1f - resistancesDmgType[(int)dmgInfo.dmgType]));
        dmg = ((dmg) * (1f - resistancesEnergy[(int)dmgInfo.energy]));
        return dmg;
    }
}