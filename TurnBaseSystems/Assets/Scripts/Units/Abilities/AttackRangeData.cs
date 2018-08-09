[System.Serializable]
public class AttackRangeData : AbilityEffect {
    public GridMask attackRange;
    public GridMask GetMask(int direction) {
        return GridMask.RotateMask(attackRange, direction);
    }
    

}
