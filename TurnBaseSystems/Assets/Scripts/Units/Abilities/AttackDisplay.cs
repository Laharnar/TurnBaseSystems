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
    //public static implicit operator Vector3(AttackDisplay atsd) {
    //    return new Vector3();
    //}

    public static void ShowGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null) {
            Debug.LogError(source + "NULL data");
            return;
        }
        if (source == null) {
            Debug.LogError("Null source");
            return;
        }
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.Instance.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(0));
        }
        if (data.standard.used) {
            if (data.standard.GetMask(0).IsSelfMask(source, attackedSlot)) {
                GridDisplay.Instance.SetUpGrid(curSlot, GridDisplayLayer.SelfActivate, data.standard.GetMask(0));
            } else {
                GridDisplay.Instance.SetUpGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(0));
            }
        }
        if (data.move.used) {
            GridDisplay.Instance.SetUpGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.Instance.SetUpGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.Instance.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.Instance.SetUpGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
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

    internal static void HideGrid(Unit curPlayerUnit, Vector3 hoveredSlot, object lastAbility) {
        throw new NotImplementedException();
    }

    public static void HideGrid(Unit source, Vector3 attackedSlot, AttackData2 data) {
        if (data == null)
            return;
        Vector3 curSlot = GridManager.SnapPoint(source.transform.position);
        if (data.range.used) {
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(Combat.Instance.mouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(Combat.Instance.mouseDirection));
        }
        if (data.move.used) {
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.Instance.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.Instance.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.mouseDirection));
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
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.range.GetMask(Combat.Instance.lastMouseDirection));
        }
        if (data.standard.used) {
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.RedAttackArea, data.standard.GetMask(Combat.Instance.lastMouseDirection));
        }
        if (data.move.used) {
            GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.GreenMovement, data.move.range);
            if (data.move.onStartApplyAOE && data.aoe.used) {
                GridDisplay.Instance.HideGrid(curSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.lastMouseDirection));
            }
            if (data.move.onEndApplyAOE && data.aoe.used) {
                GridDisplay.Instance.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.lastMouseDirection));
            }
        } else if (data.aoe.used) {
            GridDisplay.Instance.HideGrid(attackedSlot, GridDisplayLayer.OrangeAOEAttack, data.aoe.GetMask(Combat.Instance.lastMouseDirection));
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
