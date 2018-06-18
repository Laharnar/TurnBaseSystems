using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    Unit curPlayerUnit;
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
            if (NoActionsLeft()) break;
            
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
            if (curPlayerUnit && curPlayerUnit.HasActions) {
                Unit.activeUnit = curPlayerUnit;
                RecolorMap();
            }
            // -- end map changes

            bool selectionChanged = false;
            bool mousePress = Input.GetKeyDown(KeyCode.Mouse0);
            bool selectedPlayerUnit = unit && unit.flag.allianceId == 0;
            // -- Input --
            // For player units, just select it. For enemy, attack them if player unit is selected.
            if (mousePress && selectedPlayerUnit && unit != curPlayerUnit) {
                selectionChanged = true;
                DeselectUnit();
                if (selectedPlayerUnit && unit.HasActions) {
                    curPlayerUnit = unit;
                }
            }
            // attack
            if (mousePress && !selectedPlayerUnit && curPlayerUnit && curPlayerUnit.CanAttack) { // unit = enemy unit
                if (GridManager.IsUnitInAttackRange(curPlayerUnit, unit, curPlayerUnit.abilities.BasicAttack)) {
                    bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                    if (curPlayerUnit.equippedWeapon && curPlayerUnit.equippedWeapon.conePref) {
                        yield return curPlayerUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(curPlayerUnit, unit, curPlayerUnit.equippedWeapon.conePref, curPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range));
                        aimSuccesful = CheckIfEnemyHit(unit);
                    }
                    if (aimSuccesful)
                        curPlayerUnit.AttackAction(slot, unit, curPlayerUnit.abilities.BasicAttack);
                }
                yield return null;
            }
            // move
            if (Input.GetKeyDown(KeyCode.Mouse1)) {
                selectionChanged = true;
                // if unit is already selected, move to that slot
                if (slot && slot.Walkable && curPlayerUnit && curPlayerUnit.CanMoveTo(slot)) {
                    GridManager.RecolorMask(curPlayerUnit.curSlot, 0, curPlayerUnit.abilities.BasicAttack.attackMask);
                    curPlayerUnit.MoveAction(slot);
                    yield return null;
                }
            }

            // Reload env interaction buttons when player is selected.
            if (selectionChanged) {
                if (curPlayerUnit) {
                    UIInteractionController.ShowEnvInteractions(curPlayerUnit);
                } else {
                    UIInteractionController.ClearEnvInteractions();
                }
            }

            UIManager.PlayerStandardUi(!curPlayerUnit);
            UIManager.PlayerSelectAllyUnitUi(curPlayerUnit);

            // map decolor when unit run out of actions.
            if (curPlayerUnit && curPlayerUnit.NoActions) {
                DeselectUnit();
                UIInteractionController.ClearEnvInteractions();
            }

            yield return null;
            if (slot && curPlayerUnit)
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

    private bool CheckIfEnemyHit(Unit enemy) {
        bool aimSuccesful = false;
        Vector2 fireDir = WeaponFireMode.activeUnitAimDirection;
        RaycastHit2D[] hits = Physics2D.RaycastAll(curPlayerUnit.transform.position, fireDir, curPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range);
        //Debug.Log(fireDir + " "+hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            //Debug.Log(hits[i].transform.root + " "+ unit, hits[i].transform.root);
            if (hits[i].transform.root == enemy.transform) {
                aimSuccesful = true;
                break;
            }
        }
        return aimSuccesful;
    }

    private void RecolorMap() {
        GridManager.RecolorMask(curPlayerUnit.curSlot, 4, curPlayerUnit.abilities.BasicAttack.attackMask);
        curPlayerUnit.curSlot.RecolorSlot(3);
        if (!unit) // can move
            slot.RecolorSlot(1);
        else if (unit.flag.allianceId == 0) { // player, can select
            slot.RecolorSlot(3);
        } else if (unit.flag.allianceId != 0) { // enemy, maybe can attack
            slot.RecolorSlot(2);
        }
    }

    private void DeselectUnit() {
        if (curPlayerUnit) {
            curPlayerUnit.curSlot.RecolorSlot(0);
            GridManager.RecolorRange(0, GridManager.GetSlotsInMask(curPlayerUnit.gridX, curPlayerUnit.gridY, curPlayerUnit.abilities.BasicAttack.attackMask));
            curPlayerUnit = null;
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
