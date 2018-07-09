using System;
using System.Collections;
using UnityEngine;
[RequireComponent(typeof(UnitAbilities))]
public partial class Unit :MonoBehaviour, ISlotItem{
    public int gridX, gridY;
    public bool moving = false;
    public Pathing pathing;
    public Alliance flag;

    public bool IsPlayer { get { return flag.allianceId == 0; } }
    public bool IsEnemy { get { return flag.allianceId == 1; } }

    public UnitAnimations anim;


    public bool NoActions { get { return actionsLeft <= 0; } }

    public bool CanMove { get { return actionsLeft >=1; } }

    public bool HasActions { get { return !NoActions; } }

    public int hp = 5;

    public int maxActions = 2;
    int actionsLeft = 2;

    bool dead = false;

    public int maxAP = 0;
    int ap = 0;
    internal UnitAbilities abilities;
    public AiLogic ai;

    public GridItem curSlot;

    public Weapon equippedWeapon;

    public HpUIController hpUI;

    /// <summary>
    /// Used by interactions.
    /// </summary>
    [System.Obsolete("Don't use interactions.")]
    public static Unit activeUnit;

    internal int materials;

    // Collector class -- armor. It's effect is lost at beginning of next turn.
    public int temporaryArmor;
    public bool attacking = false;

    private void Start() {
        ap = maxAP;
        ResetActions();
        GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        curSlot = slot;
        Move(slot);
        FlagManager.RegisterUnit(this);

        if (!abilities) {
            abilities = GetComponent<UnitAbilities>();
        }

        if (hpUI) {
            hpUI.InitBarWithGrey(hp, 10, this);
            hpUI.ShowHpWithGrey(hp, temporaryArmor);
        }
    }

    public void OnTurnEnd() {
        if (equippedWeapon) {
            if (equippedWeapon.enhanceCounter >0) {
                equippedWeapon.enhanceCounter--;
            }
            if (equippedWeapon.enhanceCounter == 0) {
                if (equippedWeapon.enhanced)
                    equippedWeapon.enhanced.OnDeEquipEffect(equippedWeapon);
            }
        }
        if ((abilities as IEndTurnAbilities) != null) {
            AttackData[] passives = (abilities as IEndTurnAbilities).GetPassive();
            for (int i = 0; i < passives.Length; i++) {
                passives[i].ApplyDamage(this, null);
            }
        }
    }

    public void OnTurnStart() {
        ResetActions();
        ResetGreyHp();
    }
    public void AddShield(int armorAmount) {
        temporaryArmor += armorAmount;
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
        equippedWeapon = null;
        wep.dropped = false;
        wep.transform.position = otherUnit.transform.position;
        wep.transform.parent = otherUnit.transform;
        otherUnit.equippedWeapon = wep;

        PlayerFlag.m.activeAbility = abilities.BasicAttack;
    }
    public void DeEquip() {
        if (equippedWeapon) {
            equippedWeapon.transform.position = equippedWeapon.transform.position + new Vector3(0, -0.1f);
            equippedWeapon.transform.parent = null;
            equippedWeapon.dropped = true;
            equippedWeapon = null;
        }
    }
    public void Equip(Weapon wep) {
        equippedWeapon = wep;
        equippedWeapon.dropped = false;
        equippedWeapon.transform.position = transform.position;
        equippedWeapon.transform.parent = transform;

        PlayerFlag.m.activeAbility = abilities.BasicAttack;
    }

    public void MoveAction(GridItem slot) {
        if (moving) return;
        actionsLeft--;
        Move(slot);
    }

    private void Move(GridItem slot) {
        if (moving) return;
        gridX = slot.gridX;
        gridY = slot.gridY;
        pathing.GoToCoroutine(this, slot.gridX, slot.gridY);
    }
    void AttackCoroutine(AttackData attack) {
        if (attacking) return;
        if (!attack.animData.useInfo || anim==null) return;
        int attackTriggerCode = anim.TriggerToId( attack.animData.animTrigger);
        anim.SetTrigger(attackTriggerCode);
        StartCoroutine(WaitAttack(attack));
    }

    IEnumerator WaitAttack(AttackData attack) {
        attacking = true;
        yield return new WaitForSeconds( attack.animData.animLength);
        attacking = false;
    }

    internal void AttackAction(GridItem attackedSlot, Unit other, AttackData atk) {
        if (atk.requiresUnit && attackedSlot.filledBy == null) return;
        if (attacking) return;

        actionsLeft -= atk.actionCost;
        atk.ApplyDamage(this, attackedSlot);

        AttackCoroutine(atk);

        if (equippedWeapon)
            equippedWeapon.OnDamageEnhanceEffect(this, attackedSlot, other, atk);
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

    public bool CanMoveTo(GridItem slot) {
        return CanMove && (pathing.moveMask == null || GridLookup.IsSlotInMask(curSlot, slot, pathing.moveMask));
    }

    internal bool CanAttackSlot(GridItem hoveredSlot, GridMask mask) {
        return hoveredSlot && GridLookup.IsSlotInMask(this.curSlot, hoveredSlot, mask);
    }
    

    internal bool CanAttackWith(AttackData curAttack) {
        return curAttack.actionCost <= actionsLeft;
    }
}
