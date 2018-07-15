using System;
[Serializable]
public class Detection {
    public GridMask enemyDetectionMask;
    public GridMask groupSizeMask;
    internal bool detectedSomeone;

    public bool IsDetecting(Unit source, Unit other) {
        return enemyDetectionMask && GridLookup.IsSlotInMask(source.curSlot, other.curSlot, enemyDetectionMask);
    }

    public GridItem[] GetGroup(Unit source) {
        return groupSizeMask == null ? new GridItem[0] : GridAccess.OnlyEnemyUnits(GridAccess.GetSlotsInMask(source.curSlot, groupSizeMask), source.flag.allianceId);
    }
}
