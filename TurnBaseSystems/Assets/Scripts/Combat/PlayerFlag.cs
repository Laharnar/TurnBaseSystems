using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFlag : FlagController {
    
    public static PlayerFlag m;

    public static Unit selectedPlayerUnit;
    public static Unit selectedUnit;
    public static Vector3 selectedAttackSlot;

    public static Unit hoveredUnit;
    public static Unit lastHoveredUnit;
    public static Vector3 hoveredSlot;
    public static Vector3 lastHoveredSlot;

    int lastAbilityId;
    int activeAbilityId;
    //public AttackData activeAbility;
    public static AttackData2 activeAbility;

    /// <summary>
    /// Don't edit outside this script.
    /// </summary>
    public int mouseDirection = 0;

    bool MouseWheelRotate { get { return Input.GetAxis("Mouse ScrollWheel") != 0; } }

    void Reset() {
        selectedPlayerUnit = null;
        selectedUnit = null;
        hoveredUnit = null;
        lastHoveredUnit = null;
        lastAbilityId = 0;
        activeAbilityId = 0;
    }

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
        bool walkMode = false;
        while (true) {
            if (Input.GetKeyDown(KeyCode.Return)) break;

            // Swap between combat and walk
            #region Walk mode
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (!PlayerDetected())
                    walkMode = !walkMode;
                if (!walkMode) {
                    for (int i = 0; i < units.Count; i++) {
                        units[i].transform.position = GridManager.SnapPoint(units[i].transform.position);
                    }
                }
            }
            // Walk mode
            if (walkMode) {
                Vector2 moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                for (int i = 0; i < units.Count; i++) {
                    units[i].transform.Translate(moveDir);
                }
                if (PlayerDetected()) {
                    walkMode = false;
                    for (int i = 0; i < units.Count; i++) {
                        units[i].transform.position = GridManager.SnapPoint(units[i].transform.position);
                    }
                    yield return new WaitForSeconds(1);
                } else {
                    yield return null;
                    continue;
                }
            } 
            #endregion


            lastHoveredSlot = hoveredSlot;

            hoveredSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint(), true);
            if (hoveredSlot != lastHoveredSlot)
                CombatUI.OnMouseMoved();
            // show yellow slot
            CombatUI.OnMouseHoverEmpty(hoveredSlot, lastHoveredSlot);
            if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                mouseDirection = (4 + (mouseDirection + (int)f)) % 4;

                if (selectedPlayerUnit) {
                    CombatUI.OnMouseScrolled();
                    //AttackData2.HideGrid(selectedPlayerUnit, hoveredSlot, activeAbility);
                    //GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);

                    //RecalcFulters();
                    //AttackData2.ShowGrid(selectedPlayerUnit, hoveredSlot, activeAbility);
                    //GridDisplay.DisplayGrid(selectedUnit, activeAbilityId == 0 ? 1 : 2, curFilter);
                    //GridDisplay.DisplayGrid(selectedUnit, 4, curAoeFilter);
                }
            }
            WaitUnitSelection();
            
            if (Input.GetMouseButtonDown(1)) {
                selectedAttackSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint());
            }

            CombatUI.OnHover();

            if (selectedAttackSlot != null && selectedPlayerUnit!= null && Input.GetMouseButtonDown(1)) {
                if (selectedPlayerUnit.abilities.newVersion) {
                    if (activeAbility.actionCost > selectedPlayerUnit.ActionsLeft) {
                        Debug.Log("NOT enough actions. Can't attack.");
                    }
                    if (activeAbility.actionCost <= selectedPlayerUnit.ActionsLeft
                        && hoveredSlot != null 
                        && GridLookup.IsPosInMask(selectedPlayerUnit.transform.position, hoveredSlot, GetMask(0)))
                        {
                        //Debug.Log("Attacking v2 (0) " + hoveredSlot.x + " " + hoveredSlot.y);
                        CombatUI.OnBeginAttack();
                        //GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);
                        CombatManager.CombatAction(selectedPlayerUnit, hoveredSlot, activeAbility);
                        CombatManager.OnUnitExecutesAction(selectedPlayerUnit);

                        while (selectedPlayerUnit.moving) {
                            yield return null;
                        }
                        while (selectedPlayerUnit.attacking) {
                            yield return null;
                        }
                        CombatUI.OnUnitFinishesAction(selectedPlayerUnit);
                        
                        yield return null;
                    }
                }
            }

            // map decolor when unit run out of actions.
            if (selectedPlayerUnit) {
                if (selectedPlayerUnit.NoActions || !selectedPlayerUnit.CanDoAnyAction) {
                    CombatUI.OnUnitRunsOutOfActions();
                    //GridDisplay.HideGrid(selectedPlayerUnit, curFilter, curAoeFilter);
                    //ShowUI();
                }
            }

            if (MissionManager.levelCompleted) {
                break;
            }
            
            yield return null;

        }
        Reset();
        CombatUI.OnTurnComplete();
        
        
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

    private bool PlayerDetected() {
        return FlagManager.flags[1].DetectedSomeone;
    }

    GridMask GetMask(int i) {
        if (activeAbility == null)
            return null;
        if (i == 0) {
            if (activeAbility.standard.used) {
                return GridMask.RotateMask(activeAbility.AttackMask, mouseDirection);
            } else if (activeAbility.move.used) {
                return GridMask.RotateMask(activeAbility.move.range, mouseDirection);
            } else {
                return null;
            }
        }
        if (i == 1) {
            if (activeAbility.aoe.used) {
                return GridMask.RotateMask(activeAbility.aoe.aoeMask, mouseDirection);
            } else {
                return null;
            }
        }
        return null;
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

                SetActiveAbility(selectedPlayerUnit, 0);

            }
            CombatUI.OnSelectDifferentUnit();

        }
    }

    internal void SetActiveAbility(Unit unitSource, int atkId) {
        Debug.Log("Setting active ability " + atkId);
        lastAbilityId = activeAbilityId;
        activeAbilityId = atkId;
        AttackData2 lastAbility = activeAbility;
        activeAbility = unitSource.abilities.GetNormalAbilities()[atkId] as AttackData2;

        CombatUI.OnActiveAbilityChange(lastAbility, activeAbility);
    }

    private void DeselectUnit() {
        if (selectedUnit) {
            CombatUI.OnUnitDeseleted();
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
