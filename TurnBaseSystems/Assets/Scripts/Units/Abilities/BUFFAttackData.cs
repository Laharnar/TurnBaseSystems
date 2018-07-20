using System;

[System.Serializable]
public class BUFFAttackData : AttackDataType {

    public int turns = 1;
    public int armorAmt;
    public BuffType buffType = BuffType.None;
    public CombatStatus endBuffStatus = CombatStatus.Normal;
    public int[] endAnimSets;

    internal BUFFAttackData Copy() {
        BUFFAttackData buff = new BUFFAttackData();
        buff.turns = turns;
        buff.buffType = buffType;
        buff.endBuffStatus = endBuffStatus;
        buff.endAnimSets = endAnimSets;
        return buff;
    }
}
