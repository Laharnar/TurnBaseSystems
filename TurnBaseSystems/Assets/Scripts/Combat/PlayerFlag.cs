using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFlag : FlagController {
    
    public static PlayerFlag m;

    Unit selectedPlayerUnit;
    Unit selectedUnit;
    GridItem selectedSlot;

    Unit hoveredUnit;
    Unit lastHoveredUnit;
    GridItem hoveredSlot;

    Coroutine twoStepCoro;
    bool RunningTwoStepAbility { get { return twoStepCoro != null; } }

    Unit coroUnitSource;
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

            WaitMouseOverGrid();

            if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                mouseDirection = (4 + (mouseDirection + (int)f)) % 4;
                GridDisplay.HideGrid(selectedPlayerUnit, curFilter);
                GridDisplay.HideGrid(selectedPlayerUnit, curAoeFilter);
                //ResetColorForUnit(selectedPlayerUnit, curFilter);
                //ResetColorForUnit(selectedPlayerUnit, curAoeFilter);
            }
            if (hoveredSlot != null) {
                WaitUnitSelection();
            }

            // Set default action
            if (selectedPlayerUnit) {
                if (selectedPlayerUnit.HasActions && activeAbility == null) {
                    activeAbility = selectedPlayerUnit.abilities.move2;
                }
            }

            // load avaliable filter for current ability
            if ( selectedPlayerUnit) {
                if ((selectedPlayerUnit.abilities.newVersion && activeAbility != null) || activeAbility != null) {
                    curFilter = GridMask.RotateMask(activeAbility.AttackMask, mouseDirection);
                    //curFilter = GridAccess.LoadAttackLayer(selectedPlayerUnit, activeAbility.AttackMask, mouseDirection);
                    if (activeAbility.aoe.used) {
                        curAoeFilter = GridMask.RotateMask(activeAbility.aoe.aoeMask, mouseDirection);
                        //curAoeFilter = GridAccess.LoadAttackLayer(selectedPlayerUnit.curSlot, activeAbility.aoe.aoeMask, mouseDirection);
                    }
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                if (selectedSlot == null)
                    selectedSlot = GridManager.NewGridInstanceAtMouse();
                else selectedSlot.worldPosition = SelectionManager.MouseAsPos();
                //selectedSlot = SelectionManager.GetMouseAsSlot2D();
            }
            // show abilities
            if (selectedUnit) {
                ShowArea(selectedUnit, curFilter);
                ShowAoe(selectedUnit, curAoeFilter);
                //UIManager.PlayerSelectAllyUnitUi(false, selectedPlayerUnit);
                // WaitAbilitySelection(); automatic on buttons
            }
            if (hoveredSlot!=null) {
                UpdateColorsDependinOnHoveringTarget();
            }
            if (selectedSlot != null && selectedPlayerUnit!= null && Input.GetMouseButtonDown(1)) {
                if (selectedPlayerUnit.abilities.newVersion) {
                    if (activeAbility.actionCost > selectedPlayerUnit.ActionsLeft) {
                        Debug.Log("NOT enough actions. Can't attack.");
                    }
                    if (activeAbility.actionCost <= selectedPlayerUnit.ActionsLeft
                        && hoveredSlot != null && GridLookup.IsPosInMask(selectedPlayerUnit.transform.position, hoveredSlot.worldPosition, curFilter))
                        { // unit = enemy unit
                        bool isCurMouseHostile = hoveredSlot.filledBy && hoveredSlot.filledBy.flag.allianceId != selectedPlayerUnit.flag.allianceId;
                            // handle weapon aim
                        Debug.Log("Attacking v2 (0)");
                        GridDisplay.HideGrid(selectedPlayerUnit, curFilter);
                        GridDisplay.HideGrid(selectedPlayerUnit, curAoeFilter);
                        //ResetColorForUnit(selectedPlayerUnit, curFilter);
                        //ResetColorForUnit(selectedPlayerUnit, curAoeFilter);
                        selectedPlayerUnit.AttackAction2(hoveredSlot, hoveredUnit, activeAbility);
                        OnUnitExectutesAction();
                        CombatManager.OnUnitExecutesAction(selectedPlayerUnit);
                        yield return null;
                    }
                }
            }

            // map decolor when unit run out of actions.
            if (selectedUnit) {
                if (selectedUnit.NoActions || !selectedUnit.CanDoAnyAction) {
                    GridDisplay.DisplayGrid(selectedUnit.transform.position, 0, curFilter);
                    GridDisplay.DisplayGrid(selectedUnit.transform.position, 0, curAoeFilter);
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
            GridDisplay.DisplayGrid(units[i].transform.position, 0, GridMask.One);
            while (units[i].moving) {
                yield return null;
            }
            while (units[i].attacking) {
                yield return null;
            }
        }
        yield return null;
    }

    private void OnUnitExectutesAction() {
        ShowUI();// update with buttons are enabled

    }

    private void ShowUI() {
        UIManager.PlayerStandardUi(!selectedPlayerUnit && !selectedUnit);
        UIManager.PlayerSelectAllyUnitUi(selectedPlayerUnit!= null && selectedUnit!= null, selectedPlayerUnit);
    }

    private void ShowArea(Unit unit, GridMask mask) {
        if (unit) {
            GridDisplay.DisplayGrid(unit, 3, GridMask.One);
            if (unit.CanDoAnyAction) {
                bool moveVersion = activeAbilityId == 0;
                GridDisplay.DisplayGrid(unit, moveVersion ? 1 : 2, mask);
            }
        } else {
            if (selectedUnit) // an enemy
                GridDisplay.DisplayGrid(unit, 2, GridMask.One);
        }
    }
    private void ShowAoe(Unit unit, GridMask mask) {
        if (selectedPlayerUnit) {
            if (mask) {
                GridDisplay.DisplayGrid(selectedPlayerUnit, 4, mask);
            }
        }
    }
    private void WaitUnitSelection() {
        hoveredUnit = SelectionManager.GetUnitUnderMouse(hoveredSlot);
        if (Input.GetMouseButtonDown(0) && hoveredUnit && (selectedPlayerUnit == null || hoveredUnit != selectedPlayerUnit)) {
            DeselectUnit();
            selectedUnit = hoveredUnit;
            if (hoveredUnit.IsPlayer)
                selectedPlayerUnit = hoveredUnit;
            activeAbility = null;
            activeAbilityId = 0;
            ShowUI();
        }
    }

    private void WaitMouseOverGrid() {
        if (hoveredSlot == null) {
            hoveredSlot = GridManager.NewGridInstanceAtMouse();
        }
        hoveredSlot.worldPosition = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint());
        //hoveredSlot = SelectionManager.GetMouseAsSlot2D();
    }
    

    private void UpdateColorsDependinOnHoveringTarget() {
        // decolor last hovered unit
        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit && lastHoveredUnit != selectedPlayerUnit) {
            GridDisplay.DisplayGrid(lastHoveredUnit, 0, GridMask.One);
            //GridManager.RecolorSlot(0, lastHoveredUnit.curSlot);
        }
        lastHoveredUnit = hoveredUnit;
        // Color currently hovered unit depending on alliance
        if (hoveredUnit && hoveredUnit != selectedPlayerUnit){
            if (hoveredUnit.flag.allianceId == 0) { // player, can select
                GridDisplay.DisplayGrid(hoveredUnit, 3, GridMask.One);
            } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                GridDisplay.DisplayGrid(hoveredUnit, 2, GridMask.One);
            }
            //ShowArsenal
        }
    }


    internal void SetActiveAbility(Unit unitSource, int atkId) {
        activeAbilityId = atkId;
        if (activeAbility != null) {
            Debug.Log("testing as filter");

            GridDisplay.HideGrid(selectedPlayerUnit, curFilter);
            GridDisplay.HideGrid(selectedPlayerUnit, curAoeFilter);
            //ResetColorForUnit(unitSource, curFilter);
            //ResetColorForUnit(unitSource, curAoeFilter);
        }
        activeAbility = unitSource.abilities.GetNormalAbilities()[atkId] as AttackData2;
        Debug.Log("Setting active ability "+activeAbilityId);
    }
    
   /* private bool CheckIfEnemyHit(Unit enemy) {
        bool aimSuccesful = false;
        Vector2 fireDir = WeaponFireMode.activeUnitAimDirection;
        RaycastHit2D[] hits = Physics2D.RaycastAll(selectedPlayerUnit.transform.position, fireDir, selectedPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range);
        //Debug.Log(fireDir + " "+hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            //Debug.Log(hits[i].transform.root + " "+ unit, hits[i].transform.root);
            if (hits[i].transform.root == enemy.transform) {
                aimSuccesful = true;
                break;
            }
        }
        return aimSuccesful;
    }*/

    private void DeselectUnit() {
        if (selectedUnit) {
            GridDisplay.HideGrid(selectedPlayerUnit, curFilter);
            GridDisplay.HideGrid(selectedPlayerUnit, curAoeFilter);
            //ResetColorForUnit(selectedUnit, curFilter);
            //ResetColorForUnit(selectedUnit, curAoeFilter);
            selectedUnit = null;
            selectedPlayerUnit = null;
            activeAbility = null;
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

