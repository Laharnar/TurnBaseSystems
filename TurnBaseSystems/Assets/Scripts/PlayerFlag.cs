using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    Unit playerActiveUnit;

    public override IEnumerator FlagUpdate() {
        while (true) {
            if (NoActionsLeft()) {
                break;
            }
            GridItem slot = SelectionManager.GetMouseAsSlot2D();
            if (slot == null) {
                yield return null;
                continue;
            }
            Unit unit = slot.filledBy;  //SelectionManager.GetMouseAsUnit2D();
            if (playerActiveUnit) {
                if (!unit)
                    slot.RecolorSlot(1);
                else if (unit.flag.allianceId == 0) {
                    slot.RecolorSlot(3);
                } else if (unit.flag.allianceId != 0) {
                    slot.RecolorSlot(2);
                }
            }
            
            // For player units, just select it. For enemy, attack them if player unit is selected.
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                if (unit) {
                    if (unit.flag.allianceId == 0)
                        playerActiveUnit = unit;
                    else if (playerActiveUnit) { // unit = enemy unit
                        playerActiveUnit.AttackAction(slot, unit, playerActiveUnit.abilities.BasicAttack);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                // if unit is already selected, move to that slot
                if (slot && playerActiveUnit) {
                    playerActiveUnit.MoveAction(slot);
                }
            }
            UIManager.PlayerStandardUi(!playerActiveUnit);
            UIManager.PlayerSelectAllyUnitUi(playerActiveUnit);


            yield return null;
            if (slot && playerActiveUnit)
                slot.RecolorSlot(0);
        }
    }

    private bool NoActionsLeft() {
        for (int i = 0; i < units.Count; i++) {
            if (!units[i].NoActions) {
                return false;
            }
        }
        return true;
    }
}
