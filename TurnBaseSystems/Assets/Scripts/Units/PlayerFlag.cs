using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    Unit playerActiveUnit;

    public override IEnumerator FlagUpdate() {
        turnDone = false;
        NullifyUnits();
        for (int i = 0; i < units.Count; i++) {
            units[i].ResetActions();
        }
        while (true) {
            
            if (units.Count == 0) {
                Debug.Log("No units left for player.");
                break;
            }
            
            if (NoActionsLeft()) {
                break;
            }
            GridItem slot = SelectionManager.GetMouseAsSlot2D();
            if (slot == null) {
                yield return null;
                continue;
            }
            Unit unit = slot.filledBy;
            if (unit == null)
                unit = SelectionManager.GetMouseAsUnit2D();

            // -- Map recoloring  for selected unit --
            if (playerActiveUnit && !playerActiveUnit.NoActions) {
                GridManager.RecolorRange(4, GridManager.GetSlotsInMask(playerActiveUnit.gridX, playerActiveUnit.gridY, playerActiveUnit.abilities.BasicAttack.attackMask));
                playerActiveUnit.curSlot.RecolorSlot(3);
                if (!unit)
                    slot.RecolorSlot(1);
                else if (unit.flag.allianceId == 0) {
                    slot.RecolorSlot(3);
                } else if (unit.flag.allianceId != 0) {
                    slot.RecolorSlot(2);
                }
            }

            // -- Input --
            // For player units, just select it. For enemy, attack them if player unit is selected.
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                if (unit) {
                    // select
                    if (unit.flag.allianceId == 0) {
                        DeselectUnit(playerActiveUnit);
                        if (!unit.NoActions)
                            playerActiveUnit = unit;
                    }
                    // attack
                    else if (playerActiveUnit && playerActiveUnit.CanAttack) { // unit = enemy unit
                        if (GridManager.IsSlotInMask(playerActiveUnit.curSlot, unit.curSlot, playerActiveUnit.abilities.BasicAttack.attackMask))
                            playerActiveUnit.AttackAction(slot, unit, playerActiveUnit.abilities.BasicAttack);
                        GridManager.RecolorRange(0, GridManager.GetSlotsInMask(playerActiveUnit.gridX, playerActiveUnit.gridY, playerActiveUnit.abilities.BasicAttack.attackMask));
                        yield return null;
                    }
                }
            }

            // move
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                // if unit is already selected, move to that slot
                if (slot && playerActiveUnit && !slot.filledBy && playerActiveUnit.CanMove) {
                    if (playerActiveUnit)
                        playerActiveUnit.curSlot.RecolorSlot(0);
                    GridManager.RecolorRange(0, GridManager.GetSlotsInMask(playerActiveUnit.gridX, playerActiveUnit.gridY, playerActiveUnit.abilities.BasicAttack.attackMask));
                    playerActiveUnit.MoveAction(slot);
                    yield return null;
                    
                }
            }
            UIManager.PlayerStandardUi(!playerActiveUnit);
            UIManager.PlayerSelectAllyUnitUi(playerActiveUnit);

            if (playerActiveUnit && playerActiveUnit.NoActions) {
                GridManager.RecolorRange(0, GridManager.GetSlotsInMask(playerActiveUnit.gridX, playerActiveUnit.gridY, playerActiveUnit.abilities.BasicAttack.attackMask));
            }

            yield return null;
            if (slot && playerActiveUnit)
                slot.RecolorSlot(0);

            NullifyUnits();
        }
        // Wait until all actions are complete
        for (int i = 0; i < units.Count; i++) {
            units[i].curSlot.RecolorSlot(0);
            while (units[i].moving) {
                yield return null;
            }
        }
        turnDone = true;
        yield return null;
    }

    private void DeselectUnit(Unit playerActiveUnit) {
        if (playerActiveUnit)
            playerActiveUnit.curSlot.RecolorSlot(0);
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
