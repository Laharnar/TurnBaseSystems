using System;
using UnityEngine;
[System.Serializable]
public class StandardAttackData : DamageBasedAttackData {
    public int damage=1;
    public int heal = 0;
    public int selfHeal = 0;
    public DamageInfo dmgInfo = new DamageInfo(1);

    public bool usePercentDmg = false;
    /// <summary>
    /// Dmg to cur hp. 100% will 1 shot. 99% = 1 hp
    /// </summary>
    public float percentDmg = 0f;
    public bool includeArmor = true;
    public GridMask attackRangeMask;

    public int healOnKills = 0;
    public static float dmgReduction = 0;
    public TargetFilter targets = TargetFilter.All;

    public bool destroy = false;

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
                if (data.standard.percentDmg > 0f) {
                    int hp = u.hp;
                    float result = ((float)hp + (includeArmor ? (float)u.temporaryArmor : 0f)) * data.standard.percentDmg * (1f - dmgReduction);
                    CombatDebug.Log("[Std/%Dmg/CalcStep1]"+ hp + "*" +data.standard.percentDmg + "*" + (1f - dmgReduction) +"=rounded="+ result);
                    u.GetDamaged(result);
                }
            } else if (data.standard.damage > 0) {
                u.GetDamaged((int)((float)data.standard.damage * (1f - dmgReduction)));
                // add debuffs.
                AbilityInfo inf = new AbilityInfo(info) {
                    attackedSlot = u.snapPos
                };

                if (info.activeAbility.buff.used) {
                    info.activeAbility.buff.AtkBehaviourExecute(inf);
                }
                Debug.Log("After std dmg Enemy hp left: " + u.hp + " heal on kill: " + healOnKills + " healing: " + heal + " dmg: " + damage);
            }
            if (heal > 0) {
                u.Heal(heal, this);
            }
            if (selfHeal > 0) {
                info.executingUnit.Heal(selfHeal, this);
            }
            // enemy killed
            if (u.dead && healOnKills > 0) {
                info.executingUnit.Heal(healOnKills, this);
            }
            info.executingUnit.AbilitySuccess();
        }
        if (setStatus != CombatStatus.SameAsBefore)
            info.executingUnit.combatStatus = setStatus;

        if (destroy) {
            Debug.Log("Destroy triggered.");
            info.executingUnit.Die();
        }
    }
}
