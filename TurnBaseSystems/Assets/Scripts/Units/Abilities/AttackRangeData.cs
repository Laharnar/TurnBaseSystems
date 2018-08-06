﻿[System.Serializable]
public class AttackRangeData : AttackDataType {
    public GridMask attackRange;
    public GridMask GetMask(int direction) {
        return GridMask.RotateMask(attackRange, direction);
    }
}