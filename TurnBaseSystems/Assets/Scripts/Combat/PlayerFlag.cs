using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerFlag : FlagController {

    public Vector3 lastHoveredSlot { get { return PlayerTurnData.Instance.lastHoveredSlot; } set { PlayerTurnData.Instance.lastHoveredSlot = value; } }
    public Unit selectedPlayerUnit { get { return PlayerTurnData.Instance.selectedPlayerUnit; } set { PlayerTurnData.Instance.selectedPlayerUnit = value; } }
    public Unit selectedUnit { get { return PlayerTurnData.Instance.selectedUnit; } private set { PlayerTurnData.Instance.selectedUnit = value; } }
    public Vector3 selectedAttackSlot { get { return PlayerTurnData.Instance.selectedAttackSlot; } private set { PlayerTurnData.Instance.selectedAttackSlot = value; } }

    public Unit hoveredUnit { get { return PlayerTurnData.Instance.hoveredUnit; } private set { PlayerTurnData.Instance.hoveredUnit = value; } }
    public Unit lastHoveredUnit { get { return PlayerTurnData.Instance.lastHoveredUnit; } private set { PlayerTurnData.Instance.lastHoveredUnit = value; } }
    public Vector3 hoveredSlot { get { return PlayerTurnData.Instance.hoveredSlot; } private set { PlayerTurnData.Instance.hoveredSlot = value; } }

    public bool walkMode { get { return PlayerTurnData.Instance.walkMode; } private set { PlayerTurnData.Instance.walkMode = value; } }


    //bool MouseWheelRotate { get { return Input.GetAxis("Mouse ScrollWheel") != 0; } }
    //public int mouseDirection { get { return Combat.Instance.mouseDirection; } set { Combat.Instance.mouseDirection = value; } }

    public override IEnumerator FlagUpdate(FlagManager flag) {
        List<Unit> units = flag.info.units;
        while (true) {
            if (Input.GetKeyDown(KeyCode.Return)) break;

            if (MissionManager.levelCompleted) {
                break;
            }

            // Swap between combat and walk
            float f = HandleWalkMode(flag.info.units);
            if (f > 0) {
                yield return new WaitForSeconds(f);
            }
            //yield return Combat.Instance.StartCoroutine();
            if (walkMode) {
                yield return null;
                continue;
            }
            // ---- End walk
            
            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                yield return null;
                Combat.Instance.SkipWave();
            }
            
            lastHoveredSlot = hoveredSlot;
            hoveredSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint(), true);
            if (hoveredSlot != lastHoveredSlot)
                CombatUI.OnMouseMovedToDfSlot(hoveredSlot, lastHoveredSlot);

            #region Mousewheel rotate - obsolete
            /*if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                Combat.Instance.lastMouseDirection = mouseDirection;
                mouseDirection = (4 + (mouseDirection + (int)f)) % 4;

                if (selectedPlayerUnit) {
                    CombatUI.OnMouseScrolled();
                }
            }*/
            #endregion

            WaitUnitSelection();
            
            if (Input.GetMouseButtonDown(1)) {
                selectedAttackSlot = GridManager.SnapPoint(SelectionManager.GetMouseAsPoint());
            }

            CombatUI.OnHover();

            // --- ATTACKING
            if (selectedAttackSlot != null && selectedPlayerUnit != null && hoveredSlot != null
                && Input.GetMouseButtonDown(1)) {
                yield return Combat.Instance.StartCoroutine(HandleAttack());
            }

            // map decolor when unit run out of actions.
            HandleRunningOutOfActions();
            
            yield return null;
        }


        PlayerTurnData.Instance.Reset();
        CombatUI.OnTurnComplete();

        // Wait until all actions are complete
        for (int i = 0; i < units.Count; i++) {
            yield return Combat.Instance.StartCoroutine(units[i].WaitActionsToComplete());
        }
        yield return null;
    }

    private IEnumerator HandleAttack() {
        Debug.Log("Trying to attack: " + selectedAttackSlot + " in range: " +
                GridLookup.IsPosInMask(selectedPlayerUnit.transform.position, hoveredSlot, PlayerTurnData.Instance.GetMask(0)) + " enough actions:" + selectedPlayerUnit.PassGameRules(PlayerTurnData.ActiveAbility));
        //Debug.Log("Attacking v2 (0) " + hoveredSlot.x + " " + hoveredSlot.y);
        if (selectedPlayerUnit.PassGameRules(PlayerTurnData.ActiveAbility)
            && GridLookup.IsPosInMask(selectedPlayerUnit.transform.position, hoveredSlot, PlayerTurnData.Instance.GetMask(0))
            ) {

            CombatUI.OnBeginAttack();

            CombatEvents.ClickAction(selectedPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
            yield return null;
            yield return Combat.Instance.StartCoroutine(selectedPlayerUnit.WaitActionsToComplete());

            SwapToValidAbility();

            CombatUI.OnUnitFinishesAction(selectedPlayerUnit);
        }
    }


    private void HandleRunningOutOfActions() {
        if (selectedPlayerUnit) {
            if (selectedPlayerUnit.NoActions || !selectedPlayerUnit.CanDoAnyAction) {
                CombatUI.OnUnitRunsOutOfActions();
            }
        }
    }


    private float HandleWalkMode(List<Unit> units) {

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!UnitStates.DetectedSomeone(Combat.Instance.GetUnits(1)))
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
            if (UnitStates.DetectedSomeone(Combat.Instance.GetUnits(1))) {
                walkMode = false;
                for (int i = 0; i < units.Count; i++) {
                    units[i].transform.position = GridManager.SnapPoint(units[i].transform.position);
                }
                return 1f;
            }
        }
        return 0f;
    }

    private void SwapToValidAbility() {
        PlayerTurnData.Instance.lastAbility = PlayerTurnData.ActiveAbility;
        if (selectedPlayerUnit.CanDoAnyAction && 
            (PlayerTurnData.ActiveAbility==null || !selectedPlayerUnit.PassGameRules(PlayerTurnData.ActiveAbility))) {
            SetActiveAbility(selectedPlayerUnit, selectedPlayerUnit.GetNextAbilityWithEnoughActions());
        }
        CombatUI.OnActiveAbilityChange();
    }
    
    private void WaitUnitSelection() {
        hoveredUnit = GridAccess.GetUnitAtPos(hoveredSlot);// SelectionManager.GetUnitUnderMouse
        if (Input.GetMouseButtonDown(0) && hoveredUnit && (selectedPlayerUnit == null || hoveredUnit != selectedPlayerUnit)) {
            Debug.Log("selecting");
            DeselectUnit();
            selectedUnit = hoveredUnit;

            if (hoveredUnit.IsPlayer) {
                selectedPlayerUnit = hoveredUnit;

                SwapToValidAbility();
            }
            CombatUI.OnSelectDifferentUnit();

        }
    }

    internal void SetActiveAbility(Unit unitSource, AttackData2 atk) {

        Debug.Log("Setting active ability " + atk.o_attackName);
        PlayerTurnData.Instance.lastAbility = PlayerTurnData.ActiveAbility;
        PlayerTurnData.Instance.activeAbility = atk;//unitSource.abilities.GetNormalAbilities()[atkId] as AttackData2;

        CombatUI.OnActiveAbilityChange();
    }

    private void DeselectUnit() {
        if (selectedUnit) {
            CombatUI.OnUnitDeseleted();
            selectedUnit = null;
            selectedPlayerUnit = null;
            PlayerTurnData.Instance.activeAbility = null;
        }
    }

    private bool NoActionsLeft(List<Unit> units) {
        for (int i = 0; i < units.Count; i++) {
            if (!units[i].NoActions) {
                return false;
            }
        }
        return true;
    }
}
