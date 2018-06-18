using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    Unit playerActiveUnit;
    Unit unit;
    GridItem slot;

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
            slot = SelectionManager.GetMouseAsSlot2D();
            if (slot == null) {
                yield return null;
                continue;
            }
            unit = slot.filledBy;
            if (unit == null)
                unit = SelectionManager.GetMouseAsUnit2D();

            // -- Map changes for selected player unit --
            Unit.activeUnit = null;
            if (playerActiveUnit && playerActiveUnit.HasActions) {
                Unit.activeUnit = playerActiveUnit;
                // show attack range
                RecolorMap();
            }

            // -- end map changes
            bool selectionChanged = false;
            // -- Input --
            // For player units, just select it. For enemy, attack them if player unit is selected.
            if (Input.GetKeyDown(KeyCode.Mouse0) && unit && unit != playerActiveUnit) {
                selectionChanged = true;
                // select
                if (unit.flag.allianceId == 0) {
                    DeselectUnit();
                    if (unit.HasActions)
                        playerActiveUnit = unit;
                }
                // attack
                else {
                    if (playerActiveUnit && playerActiveUnit.CanAttack) { // unit = enemy unit
                        if (GridManager.IsSlotInMask(playerActiveUnit.curSlot, unit.curSlot, playerActiveUnit.abilities.BasicAttack.attackMask)) {
                            bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                            if (playerActiveUnit.equippedWeapon && playerActiveUnit.equippedWeapon.conePref) {
                                aimSuccesful = false;
                                yield return playerActiveUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(playerActiveUnit, unit, playerActiveUnit.equippedWeapon.conePref, playerActiveUnit.equippedWeapon.attackMask.Range));
                                Vector2 fireDir = WeaponFireMode.activeUnitAimDirection;
                                RaycastHit2D[] hits = Physics2D.RaycastAll(playerActiveUnit.transform.position, fireDir, playerActiveUnit.equippedWeapon.attackMask.Range);
                                //Debug.Log(fireDir + " "+hits.Length);
                                for (int i = 0; i < hits.Length; i++) {
                                    //Debug.Log(hits[i].transform.root + " "+ unit, hits[i].transform.root);
                                    if (hits[i].transform.root == unit.transform) {
                                        aimSuccesful = true;
                                        break;
                                    }
                                }
                            }
                            if (aimSuccesful)
                                playerActiveUnit.AttackAction(slot, unit, playerActiveUnit.abilities.BasicAttack);
                        }
                        yield return null;
                    }
                }       
            }
            // move
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                selectionChanged = true;
                // if unit is already selected, move to that slot
                if (slot && slot.Walkable && playerActiveUnit && playerActiveUnit.CanMove
                    && (!playerActiveUnit.pathing.moveMask || GridManager.IsSlotInMask(playerActiveUnit.curSlot, slot, playerActiveUnit.pathing.moveMask))) {
                    playerActiveUnit.MoveAction(slot);
                    yield return null;
                }
            }

            if (selectionChanged) {
                // -- ui
                if (playerActiveUnit) {
                    UIInteractionController.ShowInteractions(playerActiveUnit);
                } else {
                    UIInteractionController.HideInteractions();
                }
            }
            UIManager.PlayerStandardUi(!playerActiveUnit);
            UIManager.PlayerSelectAllyUnitUi(playerActiveUnit);

            // map decolor when unit run out of actions.
            if (playerActiveUnit && playerActiveUnit.NoActions) {
                DeselectUnit();
                UIInteractionController.HideInteractions();
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

    private void RecolorMap() {
        GridManager.RecolorMask(playerActiveUnit.curSlot, 4, playerActiveUnit.abilities.BasicAttack.attackMask);
        playerActiveUnit.curSlot.RecolorSlot(3);
        if (!unit) // can move
            slot.RecolorSlot(1);
        else if (unit.flag.allianceId == 0) { // player, can select
            slot.RecolorSlot(3);
        } else if (unit.flag.allianceId != 0) { // enemy, maybe can attack
            slot.RecolorSlot(2);
        }
    }

    private void DeselectUnit() {
        if (playerActiveUnit) {
            playerActiveUnit.curSlot.RecolorSlot(0);
            GridManager.RecolorRange(0, GridManager.GetSlotsInMask(playerActiveUnit.gridX, playerActiveUnit.gridY, playerActiveUnit.abilities.BasicAttack.attackMask));
            playerActiveUnit = null;
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
