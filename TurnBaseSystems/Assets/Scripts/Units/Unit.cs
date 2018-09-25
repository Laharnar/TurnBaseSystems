using System;
using System.Collections;
using UnityEngine;



/// <summary>
/// 
/// </summary>
/// <example>
/// Custom create unit into combat: Instantiate pref, Call init.
/// </example>
public partial class Unit : MonoBehaviour, ISlotItem{

    public string codename;
    public string description;
    public string aiDescription;
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
    public int startingArmor = 0;

    public int maxActions = 2;
    int actionsLeft = 2;
    public int maxCharges = 0;
    public int startingCharges = 1;

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

    internal Vector3 scriptedMovePos;

    public Transform characterSprite;
    public int abilitiesUsed = 0;
    public int movesUsed = 0;
    public float getDmgMult = 1f;
    public float doDmgMult = 1f;

    AttackData2 lastUsedAbility;
    public bool lastAttackParserPassed = false;
    public int reflectDmgTimes = 0;

    public Infestation infestationResult;

    private void Start() {
        Init();
    }

    public void Init() {
        if (init) return;

        ResetActions();
        //GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        if (Combat.Instance) {
            //hp = maxHp;
            AddCharges(null, startingCharges);
            stats.Increase(null, CombatStatType.Hp, maxHp);
            Vector3 snapPos = GridManager.SnapPoint(transform.position);
            //curSlot = slot;
            transform.position = snapPos;
            //Move(slot);
            Combat.Instance.RegisterUnit(this);

            if (!abilities) {
                abilities = GetComponent<UnitAbilities>();
            }
            if (hpUI) {
                if (hpUI.canvasRoot) {
                    hpUI.canvasRoot.gameObject.SetActive(hpUI.visible);
                    if (hpUI.background) hpUI.background.gameObject.SetActive(hpUI.visible);
                    hpUI.InitHiddenHpObjects(hp, 10, this);
                    hpUI.ShowHpWithGrey(hp, temporaryArmor);
                }
            }

            if (!characterSprite) {
                characterSprite = GetComponentInChildren<SpriteRenderer>().transform;
                Debug.Log(name+ " Automatically linking character sprite.", this);
            }

            if (!anim) {
                anim = GetComponentInChildren<AnimationController>();
            }

            if (ai == null) {
                ai = GetComponent<AiLogic>();
            }

            if (startingArmor > 0) {
                AddShield(null, startingArmor);
            }

            init = true;
        }
    }

    internal void AbilitySuccess() {
        lastAttackParserPassed = true;
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

    public static Unit IsUnderSomeone(Unit unit, Unit[] units) {
        for (int i = 0; i < units.Length; i++) {
            if (unit.snapPos == units[i].snapPos) {
                return units[i];
            }
        }
        return null;
    }

    public IEnumerator WaitActionsToComplete() {
        while (moving) {
            yield return null;
        }
        while (attacking) {
            yield return null;
        }
    }

    public void OnUnitTurnEnd() {
        AbilityInfo.ExecutingUnit = this;
        //AbilityInfo.Instance.activator = AbilityInfo.CurActivator.Copy();
        AbilityInfo.Instance.attackedSlot = snapPos;
        AbilityInfo.Instance.attackStartedAt = snapPos;
        Debug.Log("Applying PASSIVES.");
        for (int i = 0; i < abilities.additionalAbilities2.Count; i++) {
            AbilityInfo.ActiveAbility = abilities.additionalAbilities2[i];
            if (abilities.additionalAbilities2[i].active) {
                AttackAction(AbilityInfo.Instance, false);
                //abilities.additionalAbilities2[i].ActivateAbility(AbilityInfo.Instance);
                //AttackData2.UseAttack(this, snapPos, abilities.additionalAbilities2[i]);
                //AttackAnimations(abilities.additionalAbilities2[i]);
            }
            
        }
        //EmpowerAlliesData.DeffectEffect(snapPos, snapPos, this, AuraTrigger.OnTurnEnd);
    }

    public void AddCharges(AbilityEffect abilitySource, int amt) {
        int lastCharges = charges;
        amt = Mathf.Clamp(stats.GetSum(null, CombatStatType.Charges) + amt, 0, maxCharges);
        stats.Set(null, CombatStatType.Charges, amt);
        Debug.Log(this + " charges "+ lastCharges + " -> "+charges);
    }



    public void OnUnitTurnStart() {
        // area effect
        AbilityInfo.ExecutingUnit = this;
        AbilityInfo.Instance.activator = AbilityInfo.CurActivator.Copy();
        AbilityInfo.Instance.attackedSlot = snapPos;
        AbilityInfo.Instance.attackStartedAt = snapPos;
        Debug.Log("Applying auras on turn start for unit.");
        for (int i = 0; i < abilities.additionalAbilities2.Count; i++) {
            AbilityInfo.ActiveAbility = abilities.additionalAbilities2[i];
            if (abilities.additionalAbilities2[i].active) {
                AttackAction(AbilityInfo.Instance, false);
                //abilities.additionalAbilities2[i].ActivateAbility(AbilityInfo.Instance);
                Animations_VFX_IfParsedAttack(abilities.additionalAbilities2[i]);
                
            }
        }
        //EmpowerAlliesData.DeffectEffect(snapPos, snapPos, this, AuraTrigger.OnTurnEnd);

        // reset stats
        abilitiesUsed = 0;
        movesUsed = 0;
        ResetActions();
        attacking = false;
        lastUsedAbility = null;
        //ResetGreyHp();
    }

    public void Animations_VFX_IfParsedAttack(AttackData2 attackData2) {
        if (lastAttackParserPassed) {
            AttackAnimations(attackData2, lastAttackParserPassed);
            Combat.Instance.RunVfx(AbilityInfo.Instance.attackStartedAt, attackData2.Vfx);
            lastAttackParserPassed = false;
        }
    }
    

    /// <summary>
    /// Pass negative value, to reduce
    /// </summary>
    /// <param name="abilitySource"></param>
    /// <param name="armorAmount"></param>
    public void AddShield(AbilityEffect abilitySource, int armorAmount) {
        stats.Set(abilitySource, CombatStatType.Armor, temporaryArmor + armorAmount);

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
                if (PassGameRules(item)) {
                    return true;
                }
            }
            return false;
        }
    }

    public bool PassGameRules(AttackData2 item) {
        return ((Combat.gameRules == 4 && item.actionCost <= ActionsLeft && abilitiesUsed < 1 && (item != abilities.move2 || movesUsed < 1))
            || (Combat.gameRules == 3 && item.actionCost <= ActionsLeft && abilitiesUsed <= 1 && (item != abilities.move2 || movesUsed < 1))
            || (Combat.gameRules == 2 && item.actionCost <= ActionsLeft && abilitiesUsed <= 1)
            || (Combat.gameRules == 1 && abilitiesUsed <= 1) 
            || (Combat.gameRules == 0 && item.actionCost <= ActionsLeft))
            && !Combat.ShouldAbilityBeLocked(item.id);
    }

    internal int AttackAction(AbilityInfo info, bool costs = true) {
        AttackData2 atk = info.activeAbility;
        Vector3 attackedSlot = info.attackedSlot;

        attackedSlot = GridManager.SnapPoint(attackedSlot);

        TurnInDir(snapPos, attackedSlot);

        int r = costs ? CostsAndChecks(info) : 0;
        if (r != 0) {
            return r;
        }

        Debug.Log(info.executingUnit + " executing attack " + info.activeAbility.o_attackName + " requrements: "+info.activeAbility.requirements);

        atk.ActivateAbility(info);
        // AttackData2.UseAttack(this, attackedSlot, atk);
        lastUsedAbility = info.activeAbility;
        // AttackAnimations(atk);

        // temp
        /*if (unitType == UnitType.Pickup) {
            Destroy(gameObject);
        }*/
        return 0;
    }

    private int CostsAndChecks(AbilityInfo info) {
        Unit u = GridAccess.GetUnitAtPos(info.attackedSlot);

        if (!InteractionPass(u, info.activeAbility)) {
            return 3;
        }

        if (info.activeAbility == abilities.move2 && moving || info.activeAbility != abilities.move2 && attacking) {
            Debug.Log("Running animations, waiting. action aborted.");
            return -1;
        }

        abilitiesUsed++;
        if (info.activeAbility == abilities.move2)
            movesUsed++;

        CostActions(info.activeAbility);
        return 0;
    }


    internal AttackData2 GetLastOrNextAbilityWithEnoughActions() {
        // last used is reset on turn start
        if (lastUsedAbility != null && PassGameRules(lastUsedAbility))
            return lastUsedAbility;
        foreach (var item in abilities.GetNormalAbilities()) {
            if (PassGameRules(item)) {
                Debug.Log(item.id);
                return item;
            }
        }
        return null;
    }

    private void TurnInDir(Vector3 snapPos, Vector3 attackedSlot) {
        if (!characterSprite) {
            Debug.Log("Missing character sprite." , this);
            return;
        }
        if (snapPos.x - attackedSlot.x == 0) {
            //characterSprite.localScale = new Vector3(characterSprite, 1, 1);
        } else {
            float dir = (snapPos.x - attackedSlot.x) / Mathf.Abs(snapPos.x - attackedSlot.x);

            characterSprite.localScale = 
                new Vector3(characterSprite.localScale.x*Mathf.Sign(dir), 1, 1);
        }
    }

    public void MoveAction(Vector3 slot) {
        if (moving) return;
        //CostActions(action);
        moving = true;
        StartCoroutine(pathing.GoTo(this, slot, "Walk"));
    }

    public void Move() {
        if (moving) return;
        //CostActions(action);
        moving = true;
        StartCoroutine(pathing.GoTo(this, scriptedMovePos, "Walk"));
    }

    private void CostActions(AttackData2 atk) {
        actionsLeft -= atk.actionCost;
    }

    public void AttackAnimations(AttackData2 attack, bool attackResult) {
        if (attacking || !attackResult) return;
        if (anim == null) { Debug.Log("Can't run animations, no animator", this); return; }
        foreach (var item in attack.GetAbilityEffects()) {
            if (item.used) {
                AttackData2.RunAnimations(this, item.animSets);
            }
        }

        //float len = AttackData2.AnimLength(this, attack);
        //StartCoroutine(WaitAttack(len));
    }


    public IEnumerator WaitAttack(float len) {
        attacking = true;
        yield return new WaitForSeconds(len);
        attacking = false;
    }

    public void Heal(int amt, AbilityEffect atk) {
        stats.Set(null, CombatStatType.Hp, Mathf.Clamp(hp + amt, 0, maxHp));
        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }

    // apply all the additional stuff.
    public static int CalcDamage(Unit sourceUnit, Unit targetUnit, float dmg) {
        CombatDebug.Log("[Unit/CalcDmg] input:"+dmg + " buffBaseDmgUp:" 
            + sourceUnit.stats.GetSum(CombatStatType.StdBaseDmgUp)+" w resist: "
            + targetUnit.resistances.ApplyResistance(dmg, AbilityEffect.curDmg)
            + "final:"+" dmg: "+dmg+"*"+targetUnit.getDmgMult+"*"+sourceUnit.doDmgMult+"="+
            + (int)Math.Round((float)dmg * targetUnit.getDmgMult * sourceUnit.doDmgMult, MidpointRounding.AwayFromZero)
            , targetUnit);
        dmg += sourceUnit.stats.GetSum(CombatStatType.StdBaseDmgUp);
        dmg = targetUnit.resistances.ApplyResistance(dmg, AbilityEffect.curDmg);
        dmg = (int)Math.Round((float)dmg * targetUnit.getDmgMult * sourceUnit.doDmgMult, MidpointRounding.AwayFromZero);
        return (int)dmg;
    }

    public void GetDamaged(int realDmg) {
        GetDamaged((float)realDmg);
    }

    public void GetDamaged(float realDmg) {
        if (dead) return;
        // handle counter attack.
        if (reflectDmgTimes > 0) {
            reflectDmgTimes--;
            AttackAction(new AbilityInfo(this, abilities.additionalAbilities2[0].DefaultTarget(snapPos, AbilityInfo.Instance.executingUnit.snapPos), abilities.additionalAbilities2[0], AbilityInfo.CurActivator), false);
            return;
        }
        reflectDmgTimes = 0;

        realDmg = CalcDamage(AbilityInfo.ExecutingUnit, this, realDmg);
        int dmg = (int)realDmg;
        int dmgToHp = (int)realDmg;

        CombatDebug.Log(0, "[Unit/GetDamaged] hp" + hp + "-"+dmgToHp + "|"+dmg+"-"+temporaryArmor);
        if (dmg > 0) {
            dmgToHp = Mathf.Clamp(dmg - temporaryArmor, 0, dmg);
            //int armorLeft = Mathf.Clamp(temporaryArmor - realDmg, 0, temporaryArmor);
            stats.Reduce(CombatStatType.Armor, dmg);
            //temporaryArmor = armorLeft;
        }
        stats.Set(null, CombatStatType.Hp, Mathf.Clamp(hp - dmgToHp, 0, maxHp));
        //hp = Mathf.Clamp(hp - dmgToHp, 0, maxHp);

        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
        if (hp <= 0) {
            Die();
        }
    }

    public void Die() {
        if (dead) return;
        dead = true;
        StartCoroutine(Death());
    }

    private IEnumerator Death() {
        yield return null;
        infestationResult.InfestationEffect();
        Combat.Instance.DeRegisterUnit(this);
        MissionManager.m.RegisterDeath(this);
        Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        
    }

}

[Serializable]
public class Infestation {
    public bool spawnOnDeath;
    public int transformsOnDeath;
    public bool activated;

    public void InfestationEffect() {
        if (activated && spawnOnDeath) {
            Transform unit = CharacterLibrary.CreateInstances(new int[1] { transformsOnDeath })[0];
            unit.position = AbilityInfo.ExecutingUnit.snapPos;
            unit.GetComponent<Unit>().flag.allianceId = 0;
        }
    }
}
