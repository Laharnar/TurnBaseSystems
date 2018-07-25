using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFlag : FlagController {
    
    public static PlayerFlag m;

    Unit selectedPlayerUnit;
    Unit selectedUnit;
    Vector3 selectedSlot;

    Unit hoveredUnit;
    Unit lastHoveredUnit;
    Vector3 hoveredSlot;

    Coroutine twoStepCoro;
    bool RunningTwoStepAbility { get { return twoStepCoro != null; } }

    Unit coroUnitSource;
    int lastAbilityId;
    int activeAbilityId;
    //public AttackData activeAbility;
    public AttackData2 activeAbility;
    private GridMask curFilter;

    OffsetMask mouseToSelectOffset = new OffsetMask();
    /// <summary>
    /// Don't edit outside this script.
    /// </summary>
    public int mouseDirection = 1;
    private GridMask curAoeFilter;

    bool MouseWheelRotate { get { return Input.GetAxis("Mouse ScrollWheel") != 0; } }

    public Unit[] VisibleUnits {
        get {
            List<Unit> units1 = new List<Unit>();
            for (int i = 0; i < units.Count; i++) {
                if (units[i].combatStatus == CombatStatus.Normal)
                    units1.Add(units[i]);
            }
            return units1.ToArray();
        }
    }

    public override IEnumerator FlagUpdate() {
        m = this;
        NullifyUnits();

        while (true) {
            if (Input.GetKeyDown(KeyCode.Return)) break;
            GridDisplay.TmpHideGrid(hoveredSlot, GridMask.One);

            hoveredSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint(), true);
            GridDisplay.TmpDisplayGrid(hoveredSlot, 5, GridMask.One);
            if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                mouseDirection = (4 + (mouseDirection + (int)f)) % 4;
                GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);

                RecalcFulters();
                GridDisplay.DisplayGrid(selectedUnit, activeAbilityId== 0 ? 1 : 2, curFilter);
                GridDisplay.DisplayGrid(selectedUnit, 4, curAoeFilter);
                //ResetColorForUnit(selectedPlayerUnit, curFilter);
                //ResetColorForUnit(selectedPlayerUnit, curAoeFilter);
            }
            WaitUnitSelection();

            // load avaliable filter for current ability
            if (selectedPlayerUnit) {
                if ((selectedPlayerUnit.abilities.newVersion && activeAbility != null) || activeAbility != null) {
                    RecalcFulters();
                    
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                //if (selectedSlot == null)
                selectedSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint());
                //else selectedSlot = SelectionManager.MouseAsPos();
                //selectedSlot = SelectionManager.GetMouseAsSlot2D();
            }
            // show abilities
            if (selectedUnit) {
                GridDisplay.DisplayGrid(selectedUnit, 3, GridMask.One);
                //ShowArea(selectedUnit, curFilter);
                //ShowAoe(selectedUnit, curAoeFilter);
                //UIManager.PlayerSelectAllyUnitUi(false, selectedPlayerUnit);
                // WaitAbilitySelection(); automatic on buttons
            }
            if (lastAbilityId != activeAbilityId) {
               /* GridDisplay.HideGrid(selectedUnit, curFilter, curAoeFilter);
                lastAbilityId = activeAbilityId;
                RecalcFulters();
                GridDisplay.DisplayGrid(selectedUnit, 2, curFilter);
                GridDisplay.DisplayGrid(selectedUnit, 5, curAoeFilter);*/
            }

            OnHover(hoveredUnit);

            if (selectedSlot != null && selectedPlayerUnit!= null && Input.GetMouseButtonDown(1)) {
                if (selectedPlayerUnit.abilities.newVersion) {
                    if (activeAbility.actionCost > selectedPlayerUnit.ActionsLeft) {
                        Debug.Log("NOT enough actions. Can't attack.");
                    }
                    if (activeAbility.actionCost <= selectedPlayerUnit.ActionsLeft
                        && hoveredSlot != null && GridLookup.IsPosInMask(selectedPlayerUnit.transform.position, hoveredSlot, curFilter))
                        { 
                        Debug.Log("Attacking v2 (0)");
                        
                        //ResetColorForUnit(selectedPlayerUnit, curFilter);
                        //ResetColorForUnit(selectedPlayerUnit, curAoeFilter);
                        selectedPlayerUnit.AttackAction2(hoveredSlot, hoveredUnit, activeAbility);
                        OnUnitExectutesAction();
                        CombatManager.OnUnitExecutesAction(selectedPlayerUnit);

                        GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);

                        while (selectedPlayerUnit.moving) {
                            yield return null;
                        }
                        while (selectedPlayerUnit.attacking) {
                            yield return null;
                        }
                        if (selectedPlayerUnit.ActionsLeft >= activeAbility.actionCost) {
                            RecalcFulters();
                            GridDisplay.DisplayGrid(selectedPlayerUnit, activeAbilityId == 0 ? 1 : 2, curFilter);
                            GridDisplay.DisplayGrid(selectedPlayerUnit, 4, curAoeFilter);
                        }
                        yield return null;
                    }
                }
            }

            // map decolor when unit run out of actions.
            if (selectedUnit) {
                if (selectedUnit.NoActions || !selectedUnit.CanDoAnyAction) {
                    GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);
                    //ShowUI();
                }
            }

            if (MissionManager.levelCompleted) {
                break;
            }
            
            yield return null;

        }
        // Wait until all actions are complete
        for (int i = 0; i < units.Count; i++) {
            while (units[i].moving) {
                yield return null;
            }
            while (units[i].attacking) {
                yield return null;
            }
        }
        yield return null;
    }

    private void RecalcFulters() {
        if (activeAbility.standard.used) {
            curFilter = GridMask.RotateMask(activeAbility.AttackMask, mouseDirection);
        }
        if (activeAbility.aoe.used) {
            curAoeFilter = GridMask.RotateMask(activeAbility.aoe.aoeMask, mouseDirection);
        }
    }
    void DisplayGrid() {
        GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);

        if (selectedPlayerUnit.ActionsLeft >= activeAbility.actionCost) {
            RecalcFulters();
            GridDisplay.DisplayGrid(selectedUnit, activeAbilityId == 0 ? 1 : 2, curFilter);
            GridDisplay.DisplayGrid(selectedUnit, 4, curAoeFilter);
        }
    }
    private void OnUnitExectutesAction() {
        DisplayGrid();

        ShowUI();// update with buttons are enabled

        CombatManager.m.UnitNullCheck();
    }

    private void ShowUI() {
        UIManager.PlayerStandardUi(!selectedPlayerUnit && !selectedUnit);
        UIManager.PlayerSelectAllyUnitUi(selectedPlayerUnit!= null && selectedUnit!= null, selectedPlayerUnit);
    }

    
    private void WaitUnitSelection() {
        hoveredUnit = SelectionManager.GetUnitUnderMouse(hoveredSlot);
        if (Input.GetMouseButtonDown(0) && hoveredUnit && (selectedPlayerUnit == null || hoveredUnit != selectedPlayerUnit)) {
            DeselectUnit();
            selectedUnit = hoveredUnit;

            if (hoveredUnit.IsPlayer) {
                selectedPlayerUnit = hoveredUnit;

                lastAbilityId = activeAbilityId;
                activeAbilityId = 0;

                ShowUI();
                SetActiveAbility(selectedPlayerUnit, 0);
            }
        }
    }
    

    internal void SetActiveAbility(Unit unitSource, int atkId) {
        Debug.Log("Setting active ability "+activeAbilityId);
        lastAbilityId = activeAbilityId;
        activeAbilityId = atkId;
        activeAbility = unitSource.abilities.GetNormalAbilities()[atkId] as AttackData2;

        DisplayGrid();
    }

    void OnHover(Unit unitSource) {

        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit && lastHoveredUnit != selectedPlayerUnit) {
            GridDisplay.HideGrid(lastHoveredUnit);
            //GridManager.RecolorSlot(0, lastHoveredUnit.curSlot);
            // Color currently hovered unit depending on alliance
            if (hoveredUnit && hoveredUnit != selectedPlayerUnit) {
                if (hoveredUnit.flag.allianceId == 0) { // player, can select
                    GridDisplay.DisplayGrid(hoveredUnit, 3, GridMask.One);
                } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                    GridDisplay.DisplayGrid(hoveredUnit, 2, GridMask.One);
                }
            }
        }
        lastHoveredUnit = hoveredUnit;
    }

    private void DeselectUnit() {
        if (selectedUnit) {
            GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);
            selectedUnit = null;
            selectedPlayerUnit = null;
            activeAbility = null;
            lastAbilityId = activeAbilityId;
            activeAbilityId = 0;
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

