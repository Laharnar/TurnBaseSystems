using System;
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

    public bool IsPlayer { get { return flag.allianceId == 0; } }
    public bool IsEnemy { get { return flag.allianceId == 1; } }

    public AnimationController anim;


    public bool NoActions { get { return actionsLeft <= 0; } }

    public bool CanMove { get { return actionsLeft >=1; } }

    public bool HasActions { get { return !NoActions; } }

    public int ActionsLeft { get { return actionsLeft; } }

    public Character AsCharacterData { get { return new Character(this);  } }

    public bool CanDoAnyAction { get {
            foreach (var item in abilities.GetNormalAbilities()) {
                if (item.actionCost <= ActionsLeft) {
                    return true;
                }
            }
            return false;
        }
    }

    public int maxHp = 5;
    public int logHp;
    public int hp { get { return stats.GetSum(CombatStatType.Hp); } } //set { stats.Set(CombatStatType.Hp, value); logHp = value; } }

    public int maxActions = 2;
    int actionsLeft = 2;

    bool dead = false;

    public int maxAP = 0;
    int ap = 0;
    internal UnitAbilities abilities;
    public AiLogic ai;

    //public GridItem curSlot;

    //public Weapon equippedWeapon;

    public HpUIController hpUI;
    //internal int materials;

    public int logTemporaryArmor;
    public int temporaryArmor { get { return stats.GetSum(CombatStatType.Armor); } }// set { stats.Set(CombatStatType.Armor, value); logTemporaryArmor = value; } }

    public Vector3 snapPos { get { return GridManager.SnapPoint(transform.position); } }

    public bool attacking = false;

    bool init = false;
    public int factionId;
    internal int loyalty;

    private void Start() {
        Init();
    }
    public void Init() {
        if (init) return;

        ap = maxAP;
        ResetActions();
        //GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        if (CombatManager.m) {
            //hp = maxHp;
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
                hpUI.canvasRoot.gameObject.SetActive(true);
                hpUI.background.gameObject.SetActive(true);
                hpUI.InitBarWithGrey(hp, 10, this);
                hpUI.ShowHpWithGrey(hp, temporaryArmor);
            }

            if (!anim) {
                anim = GetComponentInChildren<AnimationController>();
            }
            init = true;
        }
    }
    public void OnTurnEnd() {
        Debug.Log("Applying passives.");
        for (int i = 0; i < abilities.additionalAbilities2.Count; i++) {
            if (abilities.additionalAbilities2[i].passive.used) {
                abilities.additionalAbilities2[i].passive.Execute(new CurrentActionData() { attackedSlot = snapPos, attackStartedAt=snapPos, sourceExecutingUnit = this }, abilities.additionalAbilities2[i]);
            }
        }
        /*if (equippedWeapon) {
            if (equippedWeapon.enhanceCounter >0) {
                equippedWeapon.enhanceCounter--;
            }
            if (equippedWeapon.enhanceCounter == 0) {
                if (equippedWeapon.enhanced)
                    equippedWeapon.enhanced.OnDeEquipEffect(equippedWeapon);
            }
        }*/
        /*if ((abilities as IEndTurnAbilities) != null) {
            AttackData[] passives = (abilities as IEndTurnAbilities).GetPassive();
            for (int i = 0; i < passives.Length; i++) {
                passives[i].ApplyDamage(this, null);
            }
        }*/
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

    public void AddShield(AttackDataType abilitySource, int armorAmount) {
        stats.Set(abilitySource, CombatStatType.Armor, temporaryArmor + armorAmount);
        

        //temporaryArmor = Mathf.Clamp(temporaryArmor + armorAmount, 0, 10);
        hpUI.ShowHpWithGrey(hp, temporaryArmor);

    }

    /*internal void RemoveShield() {
        temporaryArmor = 0;
        hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }*/
    /*private void ResetGreyHp() {
        temporaryArmor = 0;
        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }*/

    public void ResetActions(int val=-1) {
        if (val == -1)
            actionsLeft = maxActions;
        else actionsLeft = val;
    }

    public void RestoreAP(int amount) {
        if (abilities.GetType() == typeof(Collector)) {
            Debug.Log("[Collector]Restoring "+amount+" AP", this);
            // (abilities as Collector).RestoreAP(restoresAp);
            ap += amount;
            if (ap > maxAP) {
                ap = maxAP;
            }
        }
    }
    
    internal int AttackAction2(Vector3 attackedSlot, AttackData2 atk) {
        attackedSlot = GridManager.SnapPoint(attackedSlot);
        Unit u = GridAccess.GetUnitAtPos(attackedSlot);
        if (atk == abilities.move2) {
            if (u == null) {
                Debug.Log("Executing move action");
                MoveAction(attackedSlot, atk);
                return 1;
            }
        } else {
            if (attacking) {
                Debug.Log("Already attacking. action aborted.");
                return -1;
            }
            if (atk.requirements == AttackRequirments.RequiresUnit && u == null) {
                Debug.Log("This attack requires unit, no unit there. action aborted.");
                return -1;
            }
            if (atk.requirements == AttackRequirments.RequiresEmpty && u != null) {
                Debug.Log("This attack requires empty, unit is there. action aborted.");
                return -1;
            }
            Debug.Log("Executing attack " + atk.o_attackName);
            actionsLeft -= atk.actionCost;

            AttackData2.UseAttack(this, attackedSlot, atk);

            AttackAnimations(atk);
            return 2;
        }
        return -1;
    }
    public void MoveAction(Vector3 slot, AttackData2 action) {
        if (moving) return;
        CostActions(action);
        Move(slot);
    }

    private void CostActions(AttackData2 move2) {
        actionsLeft -= move2.actionCost;
    }

    public void Move(Vector3 targetPos) {
        if (moving ) return;
        StartCoroutine(pathing.GoTo(this, targetPos, "Walk"));
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

    public void GetDamaged(int realDmg) {
        if (dead) return;
        int dmgToHp = realDmg;

        if (realDmg > 0) {
            dmgToHp = Mathf.Clamp(realDmg - temporaryArmor, 0, realDmg);
            //int armorLeft = Mathf.Clamp(temporaryArmor - realDmg, 0, temporaryArmor);
            stats.Reduce(CombatStatType.Armor, realDmg);
            Debug.Log(temporaryArmor);
            //temporaryArmor = armorLeft;
        }

        stats.Set(null, CombatStatType.Hp, Mathf.Clamp(hp - dmgToHp, 0, maxHp));
        //hp = Mathf.Clamp(hp - dmgToHp, 0, maxHp);

        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
        if (hp <= 0) {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death() {
        dead = true;
        yield return null;
        FlagManager.DeRegisterUnit(this);
        Destroy(gameObject);
    }

}
