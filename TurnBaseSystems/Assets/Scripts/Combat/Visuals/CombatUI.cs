using UnityEngine;

/// <summary>
/// Updates all units in real time for quick scene tuning... save changes.
/// </summary>
public class BalanceTunning {

}

public class CombatData {
    public static CombatData Instance;

    public AttackData2 lastAbility;
    public AttackData2 activeAbility;
    public Unit selectedPlayerUnit;
    public Unit selectedUnit;
    public Vector3 selectedAttackSlot;
    public Unit hoveredUnit;
    public Unit lastHoveredUnit;
    public Vector3 hoveredSlot;
    public Vector3 lastHoveredSlot;
    public bool walkMode;

    public static AttackData2 ActiveAbility { get { return Instance.activeAbility; } }
    public static AttackData2 LastAbility { get { return Instance.lastAbility; } }


    public void Reset() {
        selectedPlayerUnit = null;
        selectedUnit = null;
        hoveredUnit = null;
        lastHoveredUnit = null;

        lastAbility = null;
        activeAbility = null;

        walkMode = false;
    }
    public GridMask GetMask(int i) {
        int mouseDirection = 0;//this.mouseDirection;
        if (activeAbility == null) {
            Debug.Log("ablity is null");
            return null;
        }
        if (i == 0) {
            if (activeAbility.range.attackRange != null) {
                return GridMask.RotateMask(activeAbility.range.attackRange, mouseDirection);
            } else
            if (activeAbility.standard.attackRangeMask != null) {
                return GridMask.RotateMask(activeAbility.standard.attackRangeMask, mouseDirection);
            } else if (activeAbility.move.range != null) {
                return GridMask.RotateMask(activeAbility.move.range, mouseDirection);
            } else {
                Debug.Log("Get mask fail 1");
                return null;
            }
        }
        if (i == 1) {
            if (activeAbility.aoe.aoeMask != null) {
                return GridMask.RotateMask(activeAbility.aoe.aoeMask, mouseDirection);
            } else {
                Debug.Log("Get mask fail 2");
                return null;
            }
        }
        return null;
    }
}

/// <summary>
/// Combat grid display.
/// </summary>
public class CombatUI {

    public static Unit lastHoveredUnit { get { return CombatData.Instance.lastHoveredUnit; } set { CombatData.Instance.lastHoveredUnit = value; } }
    public static Unit hoveredUnit { get { return CombatData.Instance.hoveredUnit; } }
    public static Unit curPlayerUnit { get { return CombatData.Instance.selectedPlayerUnit; } }
    public static Unit curUnit { get { return CombatData.Instance.selectedUnit; } }
    public static Vector3 hoveredSlot { get { return CombatData.Instance.hoveredSlot; } }
    public static Vector3 lastHoveredSlot { get { return CombatData.Instance.lastHoveredSlot; } set { CombatData.Instance.lastHoveredSlot = value; } }

    public void UpdateUI() {
        if (hoveredSlot != lastHoveredSlot)
            CombatUI.OnMouseMovedToDfSlot(hoveredSlot, lastHoveredSlot);

        CombatUI.OnHover();
    }

    internal static void OnActiveAbilityChange() {
        GridDisplay.Instance.ClearAll();
        if (CombatData.LastAbility != null)
            AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, CombatData.LastAbility);

        if (CombatData.ActiveAbility != null)
            AttackDisplay.ShowGrid(curPlayerUnit, curPlayerUnit.snapPos, CombatData.ActiveAbility);
        GridDisplay.Instance.RemakeGrid();
    }

    public static void OnSelectDifferentUnit() {
        GridDisplay.Instance.ClearAll();
        ShowUI(curPlayerUnit, curUnit, true);
        if (hoveredUnit.IsPlayer) {
            AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
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
            AttackDisplay.ShowGrid(unit, hoveredSlot, CombatData.ActiveAbility);
            GridDisplay.Instance.RemakeGrid();
        }
        ShowUI(curPlayerUnit, curUnit, true);// update with buttons are enabled
    }

    internal static void OnUnitRunsOutOfActions() {
        AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
        ShowUI(false, null, false);
        GridDisplay.Instance.RemakeGrid();
    }

    internal static void OnTurnComplete() {
        GridDisplay.Instance.ClearAll(); 
        ShowUI(curPlayerUnit, curUnit, false);
        GridDisplay.Instance.RemakeGrid();
    }

    internal static void OnMouseScrolled() {
        AttackDisplay.HideRotatedGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
        AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
        GridDisplay.Instance.RemakeGrid();
    }
    internal static void OnBeginAttack() {
        AttackDisplay.HideGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
        GridDisplay.Instance.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.BlueSelectionArea, GridMask.One);
        GridDisplay.Instance.HideGrid(curPlayerUnit.snapPos, GridDisplayLayer.RedSelectionArea, GridMask.One);

        GridDisplay.Instance.RemakeGrid();
        ShowUI(curPlayerUnit, curUnit, false);
    }

    internal static void OnMouseMovedToDfSlot(Vector3 hoveredSlot, Vector3 lastHoveredSlot) {
        if (hoveredSlot != lastHoveredSlot) {
            GridDisplay.Instance.HideGrid(lastHoveredSlot, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            AttackDisplay.HideGrid(curPlayerUnit, lastHoveredSlot, CombatData.ActiveAbility);
        }
        if (curPlayerUnit!= null && CombatData.ActiveAbility != null && hoveredSlot != lastHoveredSlot) {
            AttackDisplay.ShowGrid(curPlayerUnit, hoveredSlot, CombatData.ActiveAbility);
        }
        if (hoveredSlot != lastHoveredSlot) {
            GridDisplay.Instance.SetUpGrid(hoveredSlot, GridDisplayLayer.BlueSelectionArea, GridMask.One);
            GridDisplay.Instance.RemakeGrid();
        }
    }
}