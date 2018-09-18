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

    public int healOnKills = 0;
    public static float dmgReduction = 0;
    public TargetFilter targets = TargetFilter.All;

    internal GridMask originalRange { get; private set; }

    public Transform vfxPref;

    public void SetRange(GridMask newRange) {
        if (originalRange == null)
            originalRange = attackRangeMask;
        attackRangeMask = newRange;
    }

    public GridMask GetMask(int direction) {
        return GridMask.RotateMask(attackRangeMask, direction);
    }
    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onAttack) {
            Execute(info);
            info.executingUnit.AbilitySuccess();
        }
    }

    internal void Execute(AbilityInfo info) {
        curDmg = dmgInfo;
        dmgReduction = Mathf.Clamp(dmgReduction, 0f, 1f);
        AttackData2 data = info.activeAbility;
        Unit u = GridAccess.GetUnitAtPos(info.attackedSlot);
        if (u && EmpowerAlliesData.ValidTarget(info.executingUnit, targets, u.flag.allianceId)) {
            if (data.standard.usePercentDmg) {
                if (data.standard.percentDmg > 0f)
                    u.GetDamaged(Mathf.FloorToInt(((float)u.hp /*+ (float)u.temporaryArmor*/) * data.standard.percentDmg * (1f - dmgReduction)));
            } else if (data.standard.damage > 0) {
                u.GetDamaged((int)((float)data.standard.damage * (1f - dmgReduction)));
                // add debuffs.
                AbilityInfo inf = new AbilityInfo(info) {
                    attackedSlot = u.snapPos
                };

                if (info.activeAbility.buff.used) {
                    info.activeAbility.buff.AtkBehaviourExecute(inf);
                }

            }
            if (heal > 0) {
                u.Heal(heal, this);
            }
            // enemy killed
            Debug.Log("Enemy hp left: "+u.hp + " heal on kill: "+healOnKills+ " healing: " +heal+" dmg: "+damage);
            if (u.dead && healOnKills > 0) {
                info.executingUnit.Heal(healOnKills, this);
            }
            info.executingUnit.AbilitySuccess();
        }
        if (setStatus != CombatStatus.SameAsBefore)
            info.executingUnit.combatStatus = setStatus;
    }
}
