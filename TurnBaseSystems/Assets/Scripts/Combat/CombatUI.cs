using UnityEngine;
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
        ShowUI(curPlayerUnit, curUnit);
        if (hoveredUnit.IsPlayer) {
            AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
        }
        GridDisplay.RemakeGrid();
    }

    public static void OnHover() {
        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit) {
            GridDisplay.HideGrid(lastHoveredUnit.snapPos, 5, GridMask.One);
        }

        // Color currently hovered unit depending on alliance
        if (hoveredUnit) {
            if (hoveredUnit.flag.allianceId == 0) { // player, can select
                GridDisplay.SetUpGrid(hoveredUnit.snapPos, 5, 3, GridMask.One);
            } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                GridDisplay.SetUpGrid(hoveredUnit.snapPos, 5, 2, GridMask.One);
            }
        }
        if (curPlayerUnit != null) {
            GridDisplay.HideGrid(curPlayerUnit.snapPos, 5, GridMask.One);
            GridDisplay.SetUpGrid(curPlayerUnit.snapPos, 5, 3, GridMask.One);
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
    private static void ShowUI(Unit pUnit, Unit unit) {
        bool showPlayerUI = pUnit && unit;
        bool showButtonAbilities = pUnit != null && unit != null;
        UIManager.ShowPlayerUI(showPlayerUI);
        UIManager.ShowAbilities(showButtonAbilities, pUnit);
    }
    /// <summary>
    /// pass false and null, to hide
    /// </summary>
    /// <param name="v"></param>
    /// <param name="u"></param>
    private static void ShowUI(bool v,Unit u) {
        UIManager.ShowPlayerUI(v);
        UIManager.ShowAbilities(v, u);
    }

    internal static void OnMouseHoverEmpty(Vector3 hoveredSlot, Vector3 lastHoveredSlot) {
        //GridDisplay.HideGrid(lastHoveredSlot, 4, GridMask.One);
        //GridDisplay.SetUpGrid(hoveredSlot, 4, 5, GridMask.One);
        //GridDisplay.RemakeGrid();
    }

    internal static void OnUnitFinishesAction(Unit unit) {
        if (unit.ActionsLeft >= activeAbility.actionCost) {
            AttackData2.ShowGrid(unit, hoveredSlot, activeAbility);
            GridDisplay.RemakeGrid();
        }
        ShowUI(curPlayerUnit, curUnit);// update with buttons are enabled
    }

    internal static void OnUnitRunsOutOfActions() {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, activeAbility);
        ShowUI(false, null);
        GridDisplay.RemakeGrid();
    }

    internal static void OnTurnComplete() {
        GridDisplay.ClearAll(); 
        ShowUI(curPlayerUnit, curUnit);
        GridDisplay.RemakeGrid();
    }

    internal static void OnMouseScrolled() {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, activeAbility);
        AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
        GridDisplay.RemakeGrid();
    }
    internal static void OnBeginAttack() {
        AttackData2.HideGrid(curPlayerUnit, hoveredSlot, activeAbility);
        GridDisplay.HideGrid(curPlayerUnit.snapPos, 5, GridMask.One);

        GridDisplay.RemakeGrid();

    }

    internal static void OnMouseMoved() {
        if (curPlayerUnit!= null && activeAbility!=null) {

            AttackData2.HideGrid(curPlayerUnit, lastHoveredSlot, activeAbility);
            AttackData2.ShowGrid(curPlayerUnit, hoveredSlot, activeAbility);
            GridDisplay.RemakeGrid();
        }
    }
}