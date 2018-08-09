using System;
using UnityEngine;
[System.Serializable]
public class GridDisplayMask {
    public GridDisplayLayer layer;
    [Header("0:curSlot, 1:attackedSlot")]
    public int useSlot = 0;

    internal Vector3 GetPos() {
        return useSlot == 0 ? CI.attackStartedAt : CI.attackedSlot;
    }
}
public class AttackDisplay {

    public static void ShowGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.move.used) {
            GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.SetUpGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked != null) {
            data.pierce.Draw(attacked);
        }
    }
    public static void HideGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null)
            return;
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.move.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.mouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked != null) {
            data.pierce.Hide(attacked);
        }
    }
    public static void HideRotatedGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null)
            return;
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.move.used) {
            GridDisplay.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(CombatManager.m.lastMouseDirection));
        }
        if (data.buff.used) {
        }
        if (data.aura.used) {

        }
        Unit attacked = GridAccess.GetUnitAtPos(attackedSlot);
        if (data.pierce.used && attacked != null) {
            data.pierce.Hide(attacked);
        }
    }
}
