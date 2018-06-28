using System;
using System.Collections;
using UnityEngine;

public partial class Unit :MonoBehaviour, ISlotItem{
    public int gridX, gridY;
    public bool moving = false;
    public Pathing pathing;
    public Alliance flag;

   

    public Animator anim;


    public bool NoActions { get { return actionsLeft <= 0; } }

    public bool CanMove { get { return actionsLeft >=1; } }

    public bool HasActions { get { return !NoActions; } }

    public int hp = 5;

    public int maxActions = 2;
    int actionsLeft = 2;

    bool dead = false;

    public int maxAP = 0;
    int ap = 0;
    public UnitAbilities abilities;
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
    
    private void Start() {
        ap = maxAP;
        ResetActions();
        GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        curSlot = slot;
        Move(slot);
        FlagManager.RegisterUnit(this);

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

        PlayerFlag.m.curAttack = abilities.BasicAttack;
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

        PlayerFlag.m.curAttack = abilities.BasicAttack;
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
        pathing.GoToCoroutine(this, slot.gridX, slot.gridY, GridManager.m);
    }

    internal void AttackAction(GridItem attackedSlot, Unit other, Attack atk) {
        Debug.Log("reducing by "+atk.actionCost);
        actionsLeft-=atk.actionCost;
        atk.ApplyDamage(this, attackedSlot);

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
        return CanMove && (pathing.moveMask == null || GridManager.IsSlotInMask(curSlot, slot, pathing.moveMask));
    }

    internal bool CanAttackSlot(GridItem hoveredSlot, GridMask mask) {
        return hoveredSlot && GridManager.IsSlotInMask(this.curSlot, hoveredSlot, mask);
    }

    internal void EnvirounmentAction(GridItem hoveredSlot, Unit hoveredUnit, Attack curAttack) {
        /*if (curAttack.GetType() == typeof(PickItem)) {
            (curAttack as PickItem).ApplyDamage(this, hoveredSlot);
        }
        else */
        //curAttack.ApplyDamage(this, hoveredSlot);
        AttackAction(hoveredSlot, hoveredUnit, curAttack);
    }

    internal bool CanAttackWith(Attack curAttack) {
        return curAttack.actionCost <= actionsLeft;
    }
}
