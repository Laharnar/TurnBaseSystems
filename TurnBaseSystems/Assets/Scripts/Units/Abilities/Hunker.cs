using System;
[System.Serializable]
public class Hunker : AttackBaseType {
    public int armorAmount = 1;
    public int restoresAp = 1;
    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
        /*source.AddShield(armorAmount);
        source.RestoreAP(restoresAp);*/
    }
}
