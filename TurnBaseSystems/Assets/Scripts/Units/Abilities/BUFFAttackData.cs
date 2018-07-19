[System.Serializable]
public class BUFFAttackData : AttackDataType {

    public int turns = 1;
    public BuffType buffType = BuffType.None;
    public CombatStatus endBuffStatus = CombatStatus.Normal;
}
