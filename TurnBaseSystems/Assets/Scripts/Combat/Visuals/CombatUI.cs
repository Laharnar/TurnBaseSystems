using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Combat grid display.
/// </summary>
public class CombatUI {

    public static Unit lastHoveredUnit { get { return PlayerTurnData.Instance.lastHoveredUnit; } set { PlayerTurnData.Instance.lastHoveredUnit = value; } }
    public static Unit hoveredUnit { get { return PlayerTurnData.Instance.hoveredUnit; } }
    public static Unit curPlayerUnit { get { return PlayerTurnData.Instance.selectedPlayerUnit; } }
    public static Unit curUnit { get { return PlayerTurnData.Instance.selectedUnit; } }
    public static Vector3 hoveredSlot { get { return PlayerTurnData.Instance.hoveredSlot; } }
    public static Vector3 lastHoveredSlot { get { return PlayerTurnData.Instance.lastHoveredSlot; } set { PlayerTurnData.Instance.lastHoveredSlot = value; } }

    public void UpdateUI() {
        //if (hoveredSlot != lastHoveredSlot)
            //CombatUI.OnMouseMovedToDfSlot(hoveredSlot, lastHoveredSlot);

        //CombatUI.OnHover();
    }

    internal static void OnActiveAbilityChange() {
        GridDisplay.Instance.ClearAll();
        if (PlayerTurnData.LastAbility != null)
            AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.LastAbility);

        if (PlayerTurnData.ActiveAbility != null)
            AttackDisplay.ShowGrid(curPlayerUnit, curPlayerUnit.snapPos, PlayerTurnData.ActiveAbility);
        GridDisplay.Instance.RemakeGrid();

        ShowUI(true, curPlayerUnit, curPlayerUnit, true);
    }


    public static void OnSelectDifferentUnit() {
        GridDisplay.Instance.ClearAll();
        ShowUI(true, curPlayerUnit, curUnit, true);
        if (hoveredUnit.IsPlayer) {
            AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        }
        GridDisplay.Instance.RemakeGrid();
    }

    public static void OnHover() {
        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit) {
            GridDisplay.Instance.HideGrid(lastHoveredUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.Instance.HideGrid(lastHoveredUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);
        }

        // Color currently hovered unit depending on alliance
        if (hoveredUnit && lastHoveredUnit != hoveredUnit) {
            if (hoveredUnit.flag.allianceId == 0) { // player, can select
                GridDisplay.Instance.SetUpGrid(hoveredUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                GridDisplay.Instance.SetUpGrid(hoveredUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);
            }
        }
        if (curPlayerUnit != null) {
            GridDisplay.Instance.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.Instance.SetUpGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
        }

        lastHoveredUnit = hoveredUnit;
        GridDisplay.Instance.RemakeGrid();
    }
    

    internal static void OnUnitDeseleted() {
        GridDisplay.Instance.ClearAll();
        GridDisplay.Instance.RemakeGrid();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pUnit"></param>
    /// <param name="unit"></param>
    private static void ShowUI(bool visible, Unit pUnit, Unit unit, bool interactible) {
        bool showPlayerUI = pUnit && unit && visible;
        bool showButtonAbilities = pUnit != null && unit != null && visible;
        UIManager.ShowPlayerUI(showPlayerUI);
        UIManager.ShowAbilities(showButtonAbilities, pUnit, interactible);

        if (PlayerTurnData.ActiveAbility!= null)
            PlayerUIAbilityList.m.MarkButtonAsSelected(PlayerTurnData.ActiveAbility.id, showButtonAbilities);
        else
            PlayerUIAbilityList.m.MarkButtonAsSelected(0, false);


    }

    internal static void OnMouseHoverEmpty(Vector3 hoveredSlot, Vector3 lastHoveredSlot) {
        //GridDisplay.HideGrid(lastHoveredSlot, 4, GridMask.One);
        //GridDisplay.SetUpGrid(hoveredSlot, 4, 5, GridMask.One);
        //GridDisplay.RemakeGrid();
    }

    internal static void OnUnitFinishesAction(Unit unit) {
        if (!unit.NoActions && unit.CanDoAnyAction) {
            AttackDisplay.ShowGrid(unit, hoveredSlot, PlayerTurnData.ActiveAbility);
            GridDisplay.Instance.RemakeGrid();
        }
        ShowUI(true, curPlayerUnit, curUnit, true);// update with buttons are enabled
    }

    internal static void OnUnitRunsOutOfActions() {
        AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        ShowUI(false, curPlayerUnit, null, false);
        GridDisplay.Instance.RemakeGrid();
    }

    internal static void OnTurnComplete() {
        GridDisplay.Instance.ClearAll(); 
        ShowUI(true, curPlayerUnit, curUnit, false);
        GridDisplay.Instance.RemakeGrid();
    }

    internal static void OnMouseScrolled() {
        AttackDisplay.HideRotatedGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        GridDisplay.Instance.RemakeGrid();
    }
    internal static void OnBeginAttack() {
        AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        GridDisplay.Instance.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
        GridDisplay.Instance.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);

        GridDisplay.Instance.RemakeGrid();
        ShowUI(true, curPlayerUnit, curUnit, false);
    }

    internal static void OnMouseMovedToDfSlot(Vector3 hoveredSlot, Vector3 lastHoveredSlot) {
        if (hoveredSlot != lastHoveredSlot) {
            GridDisplay.Instance.HideGrid(lastHoveredSlot, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            if (curPlayerUnit && PlayerTurnData.ActiveAbility != null)
                AttackDisplay.HideGrid(curPlayerUnit, lastHoveredSlot, PlayerTurnData.ActiveAbility);
        }
        if (curPlayerUnit && PlayerTurnData.ActiveAbility != null && hoveredSlot != lastHoveredSlot) {
            AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, PlayerTurnData.ActiveAbility);
        }
        if (hoveredSlot != lastHoveredSlot) {
            GridDisplay.Instance.SetUpGrid(hoveredSlot, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.Instance.RemakeGrid();
        }
    }
}