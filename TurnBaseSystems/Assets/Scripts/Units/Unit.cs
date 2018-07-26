using System;
using System.Collections;
using UnityEngine;

public enum CombatStatus {
    Normal,
    Invisible,
    SameAsBefore
}
public partial class Unit : MonoBehaviour, ISlotItem{

    public string codename;
    public bool moving = false;
    public Pathing pathing;
    public Alliance flag;
    public Detection detection;
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

    public int hp = 5;

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


    internal int materials;

    // Collector class -- armor. It's effect is lost at beginning of next turn.
    public int temporaryArmor;
    public bool attacking = false;

    bool init = false;
    public int factionId;
    internal int loyalty;
    //internal GridItem curSlot;

    private void Start() {
        Init();
    }
    public void Init() {
        if (init) return;

        ap = maxAP;
        ResetActions();
        //GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        if (CombatManager.m) {
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
        ResetGreyHp();
    }
    public void AddShield(int armorAmount) {
        temporaryArmor += armorAmount;
        hpUI.ShowHpWithGrey(hp, temporaryArmor);

    }

    internal void RemoveShield() {
        temporaryArmor = 0;
        hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }
    private void ResetGreyHp() {
        temporaryArmor = 0;
        if (hpUI) hpUI.ShowHpWithGrey(hp, temporaryArmor);
    }

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

    public void EquipAction(Weapon wep) {
        Equip(wep);
    }

    public void PassWeapon(Weapon wep, Unit otherUnit) {
        //equippedWeapon = null;
        wep.dropped = false;
        wep.transform.position = otherUnit.transform.position;
        wep.transform.parent = otherUnit.transform;
        //otherUnit.equippedWeapon = wep;

        PlayerFlag.m.activeAbility = abilities.move2;
    }

    internal void AttackAction2(Vector3 attackedSlot, AttackData2 atk) {
        attackedSlot = GridManager.SnapPoint(attackedSlot);
        Unit u = GridAccess.GetUnitAtPos(attackedSlot);
        if (atk == abilities.move2) {
            if (!u) {
                Debug.Log("Executing move action");
                MoveAction(attackedSlot);
            }
        } else {
            if ((atk.requiresUnit && u == null) || attacking) {
                if (attacking) {
                    Debug.Log("Already attacking. action aborted.");
                }
                if (atk.requiresUnit && u == null) {
                    Debug.Log("This attack requires unit, no unit there. action aborted.");
                }
                return;
            }
            Debug.Log("Executing attack " + atk.o_attackName);
            actionsLeft -= atk.actionCost;

            AttackData2.UseAttack(this, attackedSlot, atk);

            AttackCoroutine2(atk);

        }
    }

    public void DeEquip() {
        /*if (equippedWeapon) {
            equippedWeapon.transform.position = equippedWeapon.transform.position + new Vector3(0, -0.1f);
            equippedWeapon.transform.parent = null;
            equippedWeapon.dropped = true;
            equippedWeapon = null;
        }*/
    }
    public void Equip(Weapon wep) {
        /*equippedWeapon = wep;
        equippedWeapon.dropped = false;
        equippedWeapon.transform.position = transform.position;
        equippedWeapon.transform.parent = transform;
        */
        PlayerFlag.m.activeAbility = abilities.move2;
    }

    public void MoveAction(Vector3 slot) {
        if (moving) return;
        actionsLeft-=abilities.move2.actionCost;
        Move(slot);
    }

    private void Move(Vector3 slot) {
        if (moving ) return;
        pathing.GoToCoroutine(this, slot);
    }

    void AttackCoroutine2(AttackData2 attack) {
        if (attacking) return;
        if (attack.standard.used == attack.aoe.used == attack.buff.used ==false|| anim == null) return;
        if (attack.standard.used) AttackData2.RunAnimations(this, attack.standard.animSets);
        if (attack.aoe.used) AttackData2.RunAnimations(this, attack.aoe.animSets);
        if (attack.buff.used) AttackData2.RunAnimations(this, attack.buff.animSets);
        float len = AttackData2.AnimLength(this, attack);
        StartCoroutine(WaitAttack(len));
    }

    void AttackCoroutine(AttackData attack) {
        if (attacking) return;
        Debug.Log("Executing atatck animation "+ attack.animData.useInfo+" "+anim );
        if (!attack.animData.useInfo || anim==null) return;
        int attackTriggerCode = anim.TriggerToId( attack.animData.animTrigger);
        anim.SetTrigger(attackTriggerCode);
        StartCoroutine(WaitAttack(attack.animData.animLength));
    }

    internal void RestoreAP(object p) {
        throw new NotImplementedException();
    }

    IEnumerator WaitAttack(float len) {
        attacking = true;
        yield return new WaitForSeconds(len);
        attacking = false;
    }

    public void GetDamaged(int realDmg) {
        if (dead) return;
        int dmgToHp = Mathf.Clamp(realDmg - temporaryArmor, 0, realDmg);
        int armorLeft = Mathf.Clamp(temporaryArmor- realDmg, 0, temporaryArmor);
        temporaryArmor = armorLeft;
        hp = hp - dmgToHp;
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
    
    

    internal bool CanAttackWith(AttackData curAttack) {
        return curAttack.actionCost <= actionsLeft;
    }
}
