using System;
using UnityEngine;

[System.Serializable]
public class StandardAttackData : DamageBasedAttackData {
    public int damage=1;
    public int heal = 0;
    public DamageInfo dmgInfo = new DamageInfo(1);

    public bool usePercentDmg = false;
    /// <summary>
    /// Dmg to cur hp. 100% will 1 shot. 99% = 1 hp
    /// </summary>
    public float percentDmg = 0f;
    [System.Obsolete]
    public GridMask attackRangeMask;
    public GridDisplayMask info;

    public int healOnKills = 0;
    public static float dmgReduction = 0;
    public AuraTarget targets = AuraTarget.All;

    public GridMask GetMask(int direction) {
        return GridMask.RotateMask(attackRangeMask, direction);
    }
    static void DrawGrid(GridMask mask, GridDisplayMask info) {
        GridDisplay.Instance.SetUpGrid(info.GetPos(), info.layer, GridMask.RotateMask(mask, Combat.Instance.mouseDirection));
    }
    static void HideGrid(GridMask mask, GridDisplayMask info) {
        GridDisplay.Instance.HideGrid(info.GetPos(), info.layer, GridMask.RotateMask(mask, Combat.Instance.mouseDirection));
    }
    internal override void Draw() {
        GridDisplay.Instance.SetUpGrid(info.GetPos(), info.layer, GetMask(Combat.Instance.mouseDirection));
    }

    internal override void Hide() {
        GridDisplay.Instance.HideGrid(info.GetPos(), info.layer, GetMask(Combat.Instance.mouseDirection));
    }

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onAttack)
            Execute(info);
    }

    internal void Execute(AbilityInfo info) {
        curDmg = dmgInfo;
        dmgReduction = Mathf.Clamp(dmgReduction, 0f, 1f);
        AttackData2 data = info.activeAbility;
        Unit u = GridAccess.GetUnitAtPos(info.attackedSlot);
        if (u && EmpowerAlliesData.ValidTarget(targets, u.flag.allianceId, info.executingUnit)) {
            if (data.standard.usePercentDmg) {
                if (data.standard.percentDmg > 0f)
                    u.GetDamaged(Mathf.FloorToInt(((float)u.hp + (float)u.temporaryArmor) * data.standard.percentDmg * (1f - dmgReduction)));
            } else if (data.standard.damage > 0)
                u.GetDamaged((int)((float)data.standard.damage * (1f - dmgReduction)));
            if (heal > 0) {
                u.Heal(heal, this);
            }
            // enemy killed
            Debug.Log("Enemy hp left "+u.hp + " heal on kill "+healOnKills+ " healing " +heal+" dmg "+damage);
            if (u.dead && healOnKills > 0) {
                info.executingUnit.Heal(healOnKills, this);
            }
        }
        if (data.standard.setStatus != CombatStatus.SameAsBefore)
            info.executingUnit.combatStatus = data.standard.setStatus;
    }
}
