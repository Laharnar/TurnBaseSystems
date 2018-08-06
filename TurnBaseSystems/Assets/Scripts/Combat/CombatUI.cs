﻿using UnityEngine;

/// <summary>
/// Updates all units in real time for quick scene tuning... save changes.
/// </summary>
public class BalanceTunning {

}

/// <summary>
/// Combat grid display.
/// </summary>
public class CombatUI {

    public static Unit lastHoveredUnit { get { return PlayerFlag.lastHoveredUnit; } set { PlayerFlag.lastHoveredUnit = value; } }
    public static Unit hoveredUnit { get { return PlayerFlag.hoveredUnit; } }
    public static Unit curPlayerUnit { get { return PlayerFlag.selectedPlayerUnit; } }
    public static Unit curUnit { get { return PlayerFlag.selectedUnit; } }
    public static AttackData2 activeAbility { get { return PlayerFlag.activeAbility; } }
    public static Vector3 hoveredSlot { get { return PlayerFlag.hoveredSlot; } }
    public static Vector3 lastHoveredSlot { get { return PlayerFlag.lastHoveredSlot; } }

    internal static void OnActiveAbilityChange(AttackData2 lastactiveAbility, AttackData2 activeAbility) {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, lastactiveAbility);

        AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
        GridDisplay.RemakeGrid();
    }

    public static void OnSelectDifferentUnit() {
        GridDisplay.ClearAll();
        ShowUI(curPlayerUnit, curUnit, true);
        if (hoveredUnit.IsPlayer) {
            AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
        }
        GridDisplay.RemakeGrid();
    }

    public static void OnHover() {
        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit) {
            GridDisplay.HideGrid(lastHoveredUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.HideGrid(lastHoveredUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);
        }

        // Color currently hovered unit depending on alliance
        if (hoveredUnit && lastHoveredUnit != hoveredUnit) {
            if (hoveredUnit.flag.allianceId == 0) { // player, can select
                GridDisplay.SetUpGrid(hoveredUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                GridDisplay.SetUpGrid(hoveredUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);
            }
        }
        if (curPlayerUnit != null) {
            GridDisplay.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.SetUpGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
        }
        lastHoveredUnit = hoveredUnit;
        GridDisplay.RemakeGrid();
    }
    

    internal static void OnUnitDeseleted() {
        GridDisplay.ClearAll();
        GridDisplay.RemakeGrid();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pUnit"></param>
    /// <param name="unit"></param>
    private static void ShowUI(Unit pUnit, Unit unit, bool interactible) {
        bool showPlayerUI = pUnit && unit;
        bool showButtonAbilities = pUnit != null && unit != null;
        UIManager.ShowPlayerUI(showPlayerUI);
        UIManager.ShowAbilities(showButtonAbilities, pUnit, interactible);
    }
    /// <summary>
    /// pass false and null, to hide
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    private static void ShowUI(bool v,Unit u, bool interactible) {
        UIManager.ShowPlayerUI(v);
        UIManager.ShowAbilities(v, u, interactible);
    }

    internal static void OnMouseHoverEmpty(Vector3 hoveredSlot, Vector3 lastHoveredSlot) {
        //GridDisplay.HideGrid(lastHoveredSlot, 4, GridMask.One);
        //GridDisplay.SetUpGrid(hoveredSlot, 4, 5, GridMask.One);
        //GridDisplay.RemakeGrid();
    }

    internal static void OnUnitFinishesAction(Unit unit) {
        if (!unit.NoActions && unit.CanDoAnyAction) {
            AttackData2.ShowGrid(unit, hoveredSlot, activeAbility);
            GridDisplay.RemakeGrid();
        }
        ShowUI(curPlayerUnit, curUnit, true);// update with buttons are enabled
    }

    internal static void OnUnitRunsOutOfActions() {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, activeAbility);
        ShowUI(false, null, false);
        GridDisplay.RemakeGrid();
    }

    internal static void OnTurnComplete() {
        GridDisplay.ClearAll(); 
        ShowUI(curPlayerUnit, curUnit, false);
        GridDisplay.RemakeGrid();
    }

    internal static void OnMouseScrolled() {
        AttackData2.HideRotatedGrid(curPlayerUnit, hoveredSlot, activeAbility);
        AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
        GridDisplay.RemakeGrid();
    }
    internal static void OnBeginAttack() {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, activeAbility);
        GridDisplay.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
        GridDisplay.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);

        GridDisplay.RemakeGrid();
        ShowUI(curPlayerUnit, curUnit, false);
    }

    internal static void OnMouseMovedToDfSlot() {
        if (curPlayerUnit!= null && activeAbility!=null) {

            AttackData2.HideGrid(curPlayerUnit, lastHoveredSlot, activeAbility);
            AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
            GridDisplay.RemakeGrid();
        }
    }
}