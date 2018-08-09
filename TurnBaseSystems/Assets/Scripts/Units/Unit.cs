﻿using System;
using System.Collections;
using UnityEngine;
public partial class Unit : MonoBehaviour, ISlotItem{

    public string codename;
    public bool moving = false;
    public Pathing pathing;
    public Alliance flag;
    public Detection detection;
    public CombatStats stats;
    public CombatStatus combatStatus;
    public UnitType unitType;

    public bool IsPlayer { get { return flag.allianceId == 0; } }
    public bool IsEnemy { get { return flag.allianceId == 1; } }

    public AnimationController anim;


    public bool NoActions { get { return actionsLeft <= 0; } }
    
    public int ActionsLeft { get { return actionsLeft; } }

    public Character AsCharacterData { get { return new Character(this);  } }


    public int maxHp = 5;
    public int logHp;
    public int hp { get { return stats.GetSum(CombatStatType.Hp); } } //set { stats.Set(CombatStatType.Hp, value); logHp = value; } }

    public int maxActions = 2;
    int actionsLeft = 2;
    public int maxCharges = 0;

    public bool dead = false;

    public int charges { get { return stats.GetSum(CombatStatType.Charges); } } //set { stats.Set(CombatStatType.Hp, value); logHp = value; } }

    internal UnitAbilities abilities;
    public AiLogic ai;

    //public GridItem curSlot;

    //public Weapon equippedWeapon;

    public HpUIController hpUI;
    //internal int materials;

    public int temporaryArmor { get { return stats.GetSum(CombatStatType.Armor); } }// set { stats.Set(CombatStatType.Armor, value); logTemporaryArmor = value; } }

    public Vector3 snapPos { get { return GridManager.SnapPoint(transform.position); } }

    public bool attacking = false;

    bool init = false;
    public int factionId;
    internal int loyalty;
    public Resistances resistances;

    private void Start() {
        Init();
    }

    public void Init() {
        if (init) return;

        ResetActions();
        //GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        if (CombatManager.m) {
            //hp = maxHp;
            AddCharges(null, 1);
            stats.Increase(null, CombatStatType.Hp, maxHp);
            Vector3 snapPos = GridManager.SnapPoint(transform.position);
            //curSlot = slot;
            transform.position = snapPos;
            //Move(slot);
            FlagManager.RegisterUnit(this);

            if (!abilities) {
                abilities = GetComponent<UnitAbilities>();
            }
            if (hpUI) {
                if (hpUI.canvasRoot) {
                    hpUI.canvasRoot.gameObject.SetActive(true);
                    if (hpUI.background) hpUI.background.gameObject.SetActive(true);
                    hpUI.InitBarWithGrey(hp, 10, this);
                    hpUI.ShowHpWithGrey(hp, temporaryArmor);
                }
            }

            if (!anim) {
                anim = GetComponentInChildren<AnimationController>();
            }
            init = true;
        }
    }
    public bool InteractionPass(Unit target, AttackData2 atk) {
        if (atk.requirements == AttackRequirments.RequiresUnit && target == null
            ) {
            Debug.Log("This attack requires unit, no unit there. action aborted.");
            return false;
        }
        if (atk.requirements == AttackRequirments.RequiresEmpty && target != null
            && target.unitType == UnitType.Normal) {
            Debug.Log("This attack requires empty, unit is there. action aborted.");
            return false;
        }
        return true;
    }

    public void OnTurnEnd() {
        CI.sourceExecutingUnit = this;
        CI.attackedSlot = snapPos;
        CI.attackStartedAt = snapPos;
        Debug.Log("Applying passives.");
        for (int i = 0; i < abilities.additionalAbilities2.Count; i++) {
            CI.activeAbility = abilities.additionalAbilities2[i];
            if (abilities.additionalAbilities2[i].passive.used) {
                AttackData2.UseAttack(this, snapPos, abilities.additionalAbilities2[i]);
                AttackAnimations(abilities.additionalAbilities2[i]);
            }
            
        }
        EmpowerAlliesData.DeffectEffect(snapPos, snapPos, this, AuraTrigger.OnTurnEnd);
    }

    public void AddCharges(AbilityEffect abilitySource, int amt) {
        amt = Mathf.Clamp(stats.GetSum(null, CombatStatType.Charges) + amt, 0, maxCharges);
        stats.Set(null, CombatStatType.Charges, amt);
    }

    public void OnTurnStart() {
        ResetActions();
        attacking = false;
        //ResetGreyHp();
    }
    /*public void AddShield(int armorAmount) {
        temporaryArmor = Mathf.Clamp(temporaryArmor + armorAmount, 0, 10);
        hpUI.ShowHpWithGrey(hp, temporaryArmor);

    }*/

    /// <summary>
    /// Pass negative value, to reduce
    /// </summary>
    /// <param name="abilitySource"></param>
    /// <param name="armorAmount"></param>
    public void AddShield(AbilityEffect abilitySource, int armorAmount) {
        stats.Set(abilitySource, CombatStatType.Armor, temporaryArmor + armorAmount);
        Debug.Log("setting armr " + temporaryArmor);


        //temporaryArmor = Mathf.Clamp(temporaryArmor + armorAmount, 0, 10);
        hpUI.ShowHpWithGrey(hp, temporaryArmor);

    }
    
    public void ResetActions(int val=-1) {
        if (val == -1)
            actionsLeft = maxActions;
        else actionsLeft = val;
    }


    public bool CanDoAnyAction {
        get {
            foreach (var item in abilities.GetNormalAbilities()) {
                if (item.actionCost <= ActionsLeft) {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Indiscriminately run all possible abilities.
    /// </summary>
    /// <param name="activator"></param>
    public void RunAllAbilities(CombatEventMask activator) {
        CI.sourceSecondaryExecUnit = this;
        for (int i = 0; i < abilities.additionalAbilities2.Count; i++) {
            for (int j = 0; j < abilities.additionalAbilities2[i].effects.Length
                && j < abilities.additionalAbilities2[i].activators.Length; j++) {
                if (CombatEventMask.CanActivate(activator, abilities.additionalAbilities2[i].activators[j]))
                {
                    abilities.additionalAbilities2[i].effects[j].Execute(abilities.atkLib, activator);
                }
            }
        }
    }

    internal int AttackAction2(Vector3 attackedSlot, AttackData2 atk) {
        attackedSlot = GridManager.SnapPoint(attackedSlot);
        Unit u = GridAccess.GetUnitAtPos(attackedSlot);
        TurnInDir(snapPos, attackedSlot);
        if (!InteractionPass(u, atk)) {
            return 3;
        }

        if (atk == abilities.move2 && moving || atk != abilities.move2 && attacking) {
            Debug.Log("Running animations, waiting. action aborted.");
            return -1;
        }
        Debug.Log("Executing attack " + atk.o_attackName);

        CostActions(atk);

        AttackData2.UseAttack(this, attackedSlot, atk);

        AttackAnimations(atk);

        // temp
        if (unitType == UnitType.Pickup) {
            Destroy(gameObject);
        }
        return 2;
    }

    private void TurnInDir(Vector3 snapPos, Vector3 attackedSlot) {
        if (snapPos.x - attackedSlot.x == 0)
            transform.localScale = new Vector3(1, 1, 1);
        else {
            float dir = (snapPos.x - attackedSlot.x) / Mathf.Abs(snapPos.x - attackedSlot.x);

            transform.localScale = new Vector3(dir, 1, 1);
        }
    }

    public void MoveAction(Vector3 slot) {
        if (moving) return;
        //CostActions(action);
        StartCoroutine(pathing.GoTo(this, slot, "Walk"));
    }

    private void CostActions(AttackData2 atk) {
        actionsLeft -= atk.actionCost;
    }

    void AttackAnimations(AttackData2 attack) {
        if (attacking) return;
        if (anim == null) { Debug.Log("Can't run animations, no animator", this); return; }
        if (!attack.standard.used && !attack.aoe.used && !attack.buff.used) return;
        if (attack.standard.used) AttackData2.RunAnimations(this, attack.standard.animSets);
        if (attack.aoe.used) AttackData2.RunAnimations(this, attack.aoe.animSets);
        if (attack.buff.used) { AttackData2.RunAnimations(this, attack.buff.animSets); }
        float len = AttackData2.AnimLength(this, attack);
        StartCoroutine(WaitAttack(len));
    }

    IEnumerator WaitAttack(float len) {
        attacking = true;
        yield return new WaitForSeconds(len);
        attacking = false;
    }

    public void Heal(int amt, AbilityEffect atk) {
        stats.Set(null, CombatStatType.Hp, Mathf.Clamp(hp + amt, 0, maxHp));
        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }
    public void GetDamaged(int realDmg) {
        if (dead) return;
        realDmg = resistances.ApplyResistance(realDmg, AbilityEffect.curDmg);
        int dmgToHp = realDmg;

        if (realDmg > 0) {
            dmgToHp = Mathf.Clamp(realDmg - temporaryArmor, 0, realDmg);
            //int armorLeft = Mathf.Clamp(temporaryArmor - realDmg, 0, temporaryArmor);
            stats.Reduce(CombatStatType.Armor, realDmg);
            //temporaryArmor = armorLeft;
        }

        stats.Set(null, CombatStatType.Hp, Mathf.Clamp(hp - dmgToHp, 0, maxHp));
        //hp = Mathf.Clamp(hp - dmgToHp, 0, maxHp);

        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
        if (hp <= 0) {
            dead = true;
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death() {
        yield return null;
        FlagManager.DeRegisterUnit(this);
        Destroy(gameObject); 
    }

}
