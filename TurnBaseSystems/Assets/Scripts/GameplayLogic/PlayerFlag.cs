using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    public static PlayerFlag m;

    Unit curPlayerUnit;
    Unit unit;
    GridItem slot;

    Coroutine twoStepCoro;

    bool RunningAbility { get { return twoStepCoro != null; } }

    GridMask curMask;

    public override IEnumerator FlagUpdate() {
        m = this;
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

            if (Input.GetKeyDown(KeyCode.Escape)) {
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
            if (curPlayerUnit && curPlayerUnit.HasActions) {
                Unit.activeUnit = curPlayerUnit;
                curMask = curPlayerUnit.abilities.BasicMask;
                //RemaskActive(3);
                if (!unit) { // can move
                    // RemaskUnit(1);
                } else if (unit.flag.allianceId == 0) { // player, can select
                                                        //  RemaskUnit(3);
                } else if (unit.flag.allianceId != 0) { // enemy, maybe can attack
                                                        //RemaskUnit(2);
                }
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

                if (curPlayerUnit) {
                    PlayerUIAbilityList.AssignAbilitiesToUI(curPlayerUnit.abilities);
                }
            }

            if (curPlayerUnit) {
                curMask = curPlayerUnit.pathing.moveMask;
                RemaskActive(3);
                curMask = curPlayerUnit.abilities.BasicMask;
                RemaskActive(2);
                RemaskUnit(1);
            }
            // move
            if (Input.GetKeyDown(KeyCode.Mouse1) && !RunningAbility) {
                selectionChanged = true;
                // if unit is already selected, move to that slot
                if (slot && slot.Walkable && curPlayerUnit && curPlayerUnit.CanMoveTo(slot)) {
                    curMask = curPlayerUnit.abilities.BasicMask;

                    curMask = curPlayerUnit.pathing.moveMask;
                    RemaskActive(0);
                    curMask = curPlayerUnit.abilities.BasicMask;
                    RemaskActive(0);
                    RemaskUnit(0);

                    curPlayerUnit.MoveAction(slot);
                    yield return null;
                }
            }
            // attack
            if (!RunningAbility && mousePress && !selectedPlayerUnit && curPlayerUnit && curPlayerUnit.CanAttack) { // unit = enemy unit
                if (GridManager.IsUnitInAttackRange(curPlayerUnit, unit, curPlayerUnit.abilities.BasicAttack)) {
                    bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                    if (curPlayerUnit.equippedWeapon && curPlayerUnit.equippedWeapon.conePref) {
                        yield return curPlayerUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(curPlayerUnit, unit, curPlayerUnit.equippedWeapon.conePref, curPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range));
                        aimSuccesful = CheckIfEnemyHit(unit);
                    }
                    if (aimSuccesful) {
                        curPlayerUnit.AttackAction(slot, unit, curPlayerUnit.abilities.BasicAttack);
                        curMask = curPlayerUnit.abilities.BasicMask;

                        curMask = curPlayerUnit.pathing.moveMask;
                        RemaskActive(0);
                        curMask = curPlayerUnit.abilities.BasicMask;
                        RemaskActive(0);
                        RemaskUnit(0);
                    }
                }
                yield return null;
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
 

    internal void StartTwoStepAttack(UnitAbilities unitSource, int atkId) {
        if (twoStepCoro != null)
            unitSource.StopCoroutine(twoStepCoro);
        twoStepCoro = unitSource.StartCoroutine(HoldAttackForUserInput(unitSource, atkId));
    }

    IEnumerator HoldAttackForUserInput(UnitAbilities source, int atkId) {
        yield return null;
        while (true) {
            if (Input.GetKeyUp(KeyCode.Mouse0)) {
                break;
            }
            yield return null;
        }
        source.GetNormalAbilities()[atkId].ApplyDamage(curPlayerUnit, unit.curSlot);
        twoStepCoro = null;
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


    /// <param name="color">0: normal, 1: selected, 2: attackable, 3: ally</param>
    void RemaskUnit(int color) {
        curPlayerUnit.curSlot.RecolorSlot(color);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color">0: normal, 1: selected, 2: attackable, 3: ally</param>
    void RemaskActive(int color) {
        if (curMask)
            GridManager.RecolorMask(curPlayerUnit.curSlot, color, curMask);
        else Debug.Log("Warning: mask is not assigned.");
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

