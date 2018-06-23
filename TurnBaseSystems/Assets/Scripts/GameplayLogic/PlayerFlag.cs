using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    public static PlayerFlag m;

    Unit curPlayerUnit;
    Unit curUnit;
    Unit lastUnit;
    GridItem curSlot;

    Coroutine twoStepCoro;
    bool RunningTwoStepAlt { get { return twoStepCoro != null; } }

    GridMask curMask;

    Unit coroUnitSource;
    int coroAtkId;
    public Attack curAttack;

    public override IEnumerator FlagUpdate() {
        m = this;
        turnDone = false;
        NullifyUnits();
        for (int i = 0; i < units.Count; i++) {
            units[i].ResetActions();
        }
        while (true) {
            if (units.Count == 0 || NoActionsLeft() || Input.GetKeyDown(KeyCode.Escape)) break;

            curSlot = SelectionManager.GetMouseAsSlot2D();
            if (curSlot == null) { // didn't hover over map
                yield return null;
                continue;
            }
            curUnit = GetUnitUnderMouse();


            bool selectionChanged = false;
            bool mousePress = Input.GetKeyDown(KeyCode.Mouse0);
            bool mouse2Press = Input.GetKeyDown(KeyCode.Mouse1);
            bool selectedPlayerUnit = curUnit && curUnit.flag.allianceId == 0;
            bool selectedDifferentUnit = curUnit != curPlayerUnit;


            if (RunningTwoStepAlt) {
                if (mousePress) {
                    curPlayerUnit.AttackAction(curSlot, curUnit, coroUnitSource.abilities.GetNormalAbilities()[coroAtkId]);
                    coroUnitSource.StopCoroutine(twoStepCoro);
                    twoStepCoro = null;
                }
            }

            // -- Input --
            // For player units, just select it. For enemy, attack them if player unit is selected.
            if (mousePress && selectedPlayerUnit && selectedDifferentUnit) {
                selectionChanged = true;
                DeselectUnit();

                if (curUnit.HasActions) {
                    curPlayerUnit = curUnit;
                    PlayerUIAbilityList.LoadAbilitiesOnUI(curPlayerUnit);
                    curAttack = curPlayerUnit.abilities.BasicAttack;
                    ShowArsenal(curPlayerUnit, curPlayerUnit.abilities.BasicMask);
                    
                }
            }

            // -- Map changes for hovered unit --
            if (lastUnit && lastUnit!= curUnit) {
                lastUnit.curSlot.RecolorSlot(0);
            }
            Unit.activeUnit = null;
            if (curPlayerUnit && curPlayerUnit.HasActions) {
                Unit.activeUnit = curPlayerUnit;
                ShowArsenal(curPlayerUnit, curAttack.attackMask);
            }
            if (curUnit) {
                if (curUnit.flag.allianceId == 0) { // player, can select
                    curUnit.curSlot.RecolorSlot(3);
                } else if (curUnit.flag.allianceId != 0) { // enemy, maybe can attack
                    curUnit.curSlot.RecolorSlot(2);
                }
                lastUnit = curUnit;
                //ShowArsenal
            }
            // -- end map changes


            // move
            if (mouse2Press && !selectedPlayerUnit) {
                if (RunningTwoStepAlt) {
                    coroUnitSource.StopCoroutine(twoStepCoro);
                    twoStepCoro = null;
                }

                selectionChanged = true;
                MoveCurToSelectedSlot();
                yield return null;
            }

            // attack
            else if (mousePress && !selectedPlayerUnit) { // unit = enemy unit
                
                if (curPlayerUnit && curPlayerUnit.CanAttack && GridManager.IsUnitInAttackRange(curPlayerUnit, curUnit, curPlayerUnit.abilities.BasicAttack)) {
                    bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                    
                    // handle weapon aim
                    if (curPlayerUnit.equippedWeapon && curPlayerUnit.equippedWeapon.conePref) {
                        yield return curPlayerUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(curPlayerUnit, curUnit, curPlayerUnit.equippedWeapon.conePref, curPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range));
                        aimSuccesful = CheckIfEnemyHit(curUnit);
                    }

                    if (aimSuccesful) {
                        curPlayerUnit.AttackAction(curSlot, curUnit, curPlayerUnit.abilities.BasicAttack);
                        
                        ResetColorForUnit(curPlayerUnit);
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

    private void ShowArsenal(Unit unit, GridMask attackMask) {
        curMask = unit.pathing.moveMask;
        RemaskActive(1);
        curMask = attackMask;
        RemaskActive(2);
        unit.curSlot.RecolorSlot(3);
    }

    private Unit GetUnitUnderMouse() {
        Unit cur = curSlot.filledBy;
        if (cur == null) // maybe hovering over unit's head, which is in other slot.
            cur = SelectionManager.GetMouseAsUnit2D();
        return cur;
    }

    private void MoveCurToSelectedSlot() {
        if (curSlot && curSlot.Walkable && curPlayerUnit && curPlayerUnit.CanMoveTo(curSlot)) {
            ResetColorForUnit(curPlayerUnit);

            curPlayerUnit.MoveAction(curSlot);
        }
    }

    private void ResetColorForUnit(Unit unit) {
        curMask = unit.pathing.moveMask;
        RemaskActive(0);
        curMask = unit.abilities.BasicMask;
        RemaskActive(0);
        unit.curSlot.RecolorSlot(0);
    }

    internal void StartTwoStepAttack(Unit unitSource, int atkId) {
        if (twoStepCoro != null)
            unitSource.StopCoroutine(twoStepCoro);
        
        twoStepCoro = unitSource.StartCoroutine(HoldAttackForUserInput(unitSource, atkId));
    }

    IEnumerator HoldAttackForUserInput(Unit source, int atkId) {
        coroUnitSource = source;
        coroAtkId = atkId;
        
        curAttack = coroUnitSource.abilities.GetNormalAbilities()[coroAtkId];
        ResetColorForUnit(coroUnitSource);
        ShowArsenal(coroUnitSource, coroUnitSource.abilities.GetNormalAbilities()[coroAtkId].attackMask);
        
        yield return null;
        while (true) {
            yield return null;
        }
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
            ResetColorForUnit(curPlayerUnit);
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

