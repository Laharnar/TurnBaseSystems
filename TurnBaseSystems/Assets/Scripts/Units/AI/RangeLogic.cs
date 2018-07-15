using System.Collections;
using UnityEngine;
public class DebugGrid{
    public static IEnumerator BlinkColor(params GridItem[] grids) {
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(0);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(5);
        }
        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < grids.Length; i++) {
            grids[i].RecolorSlot(0);
        }

    }
}

public class RangeLogic : AiLogic {

    public override IEnumerator Execute(Unit unit) {
        // command 1.
        PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;
        

        if (!unit.detection.detectedSomeone)
            yield break;

        float[] dists = transform.position.GetDistances(pFlag.units.ToArray());
        int closestUnitIndex = dists.GetIndexOfMin();
        GridItem closestUnit = pFlag.units[closestUnitIndex].curSlot;
        GridItem nearbySlot = unit.curSlot;
        if (!GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.BasicMask)) {
            nearbySlot = AiHelper.ClosestToAttackEdgeOverMoveMask(unit.curSlot, closestUnit, unit.pathing.moveMask, unit.abilities.BasicAttack.attackMask);

            if (nearbySlot == null)
                yield break;

            yield return unit.StartCoroutine(DebugGrid.BlinkColor(nearbySlot));
                
            unit.MoveAction(nearbySlot);
            while (unit.moving) {
                yield return null;
            }
        }
        // command 2
        if (GridLookup.IsSlotInMask(nearbySlot, closestUnit, unit.abilities.BasicMask)) {
            yield return unit.StartCoroutine(DebugGrid.BlinkColor(closestUnit));

            unit.AttackAction(closestUnit, pFlag.units[closestUnitIndex], unit.abilities.BasicAttack);
        }

        while (unit.attacking) {
            yield return null;
        }
        // end unit turn
        yield return null;
    }
}