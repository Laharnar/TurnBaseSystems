using System;
using UnityEngine;

[Serializable]
[System.Obsolete("Detection isn't used atm")]
public class Detection {
    public GridMask enemyDetectionMask;
    public GridMask groupSizeMask;
    internal bool detectedSomeone;

    public bool IsDetecting(Unit source, Unit other) {
        return other.combatStatus!= CombatStatus.Invisible && enemyDetectionMask && enemyDetectionMask.IsPosInMask(source.transform.position, other.transform.position);
    }

    public Unit[] GetGroup(Unit source) {
        if (groupSizeMask == null)
            return null;
        return  GridAccess.OnlyAlliedUnits(groupSizeMask.GetPositions(source.transform.position), source.flag.allianceId);
    }
}
