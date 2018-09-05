using UnityEngine;

public class PlayerTurnData {
    /// <summary>
    /// Info about the current state of the combat FOR PLAYER.
    /// </summary>
    public static PlayerTurnData Instance;

    public AttackData2 lastAbility;
    public AttackData2 ActiveAbility { get { return AbilityInfo.Instance.activeAbility; } set { AbilityInfo.Instance.activeAbility = value; } }
    public Unit selectedPlayerUnit;
    public Unit selectedUnit;
    public Vector3 selectedAttackSlot;
    public Unit hoveredUnit;
    public Unit lastHoveredUnit;
    public Vector3 hoveredSlot;
    public Vector3 lastHoveredSlot;
    public bool walkMode;

    public PlayerTurnData(PlayerTurnData data) {
        this.lastAbility = data.lastAbility;
        //this.ActiveAbility = data.ActiveAbility;
        this.selectedPlayerUnit = data.selectedPlayerUnit;
        this.selectedUnit = data.selectedUnit;
        this.selectedAttackSlot = data.selectedAttackSlot;
        this.hoveredUnit = data.hoveredUnit;
        this.lastHoveredUnit = data.lastHoveredUnit;
        this.hoveredSlot = data.hoveredSlot;
        this.lastHoveredSlot = data.lastHoveredSlot;
        this.walkMode = data.walkMode;
    }

    public PlayerTurnData() {
    }

    //public static AttackData2 ActiveAbility { get { return Instance.ActiveAbility; } }
    public static AttackData2 LastAbility { get { return Instance.lastAbility; } }


    public void Reset() {
        selectedPlayerUnit = null;
        selectedUnit = null;
        hoveredUnit = null;
        lastHoveredUnit = null;

        lastAbility = null;
        ActiveAbility = null;

        walkMode = false;
    }
    public GridMask GetMask(int i) {
        int mouseDirection = 0;//this.mouseDirection;
        if (ActiveAbility == null) {
            Debug.Log("ablity is null");
            return null;
        }
        if (i == 0) {
            if (ActiveAbility.range.attackRange != null) {
                return GridMask.RotateMask(ActiveAbility.range.attackRange, mouseDirection);
            } else
            if (ActiveAbility.standard.attackRangeMask != null) {
                return GridMask.RotateMask(ActiveAbility.standard.attackRangeMask, mouseDirection);
            } else if (ActiveAbility.move.range != null) {
                return GridMask.RotateMask(ActiveAbility.move.range, mouseDirection);
            } else {
                Debug.Log("Get mask fail 1");
                return null;
            }
        }
        if (i == 1) {
            if (ActiveAbility.aoe.aoeMask != null) {
                return GridMask.RotateMask(ActiveAbility.aoe.aoeMask, mouseDirection);
            } else {
                Debug.Log("Get mask fail 2");
                return null;
            }
        }
        return null;
    }

    internal PlayerTurnData Copy() {
        return new PlayerTurnData(this);
    }
}
