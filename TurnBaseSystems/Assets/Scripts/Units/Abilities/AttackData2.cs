using System.Collections.Generic;
[System.Serializable]
public sealed class AttackData2: StdAttackData {
    public string o_attackName;
    public int actionCost = 1;
    public bool requiresUnit = true;
    public StandardAttackData standard;
    public AOEAttackData aoe;
    public BUFFAttackData buff;

    public AttackDataType[] GetAttacks() {
        AttackDataType[] attacks = new AttackDataType[3];
        if (standard.priority == -1) {
            standard.priority = 0;
        }
        if (aoe.priority != -1) {
            aoe.priority = 0;
        }
        if (buff.priority != -1) {
            buff.priority = 0;
        }
        attacks[standard.priority] = standard;
        attacks[aoe.priority] = aoe;
        attacks[buff.priority] = buff;
        return attacks;
    }
}
