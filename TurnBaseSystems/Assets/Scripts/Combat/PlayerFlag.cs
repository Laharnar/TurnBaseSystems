using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    public static PlayerFlag m;

    Unit selectedPlayerUnit;
    Unit selectedUnit;
    GridItem selectedSlot;

    Unit hoveredUnit;
    Unit lastHoveredUnit;
    GridItem hoveredSlot;

    Coroutine twoStepCoro;
    bool RunningTwoStepAbility { get { return twoStepCoro != null; } }

    Unit coroUnitSource;
    int activeAbilityId;
    //public AttackData activeAbility;
    public AttackData2 activeAbility;
    private GridMask curFilter;

    OffsetMask mouseToSelectOffset = new OffsetMask();
    /// <summary>
    /// Don't edit outside this script.
    /// </summary>
    public int mouseDirection = 1;

    bool MousePress { get { return Input.GetKeyDown(KeyCode.Mouse0); } }
    bool Mouse2Press { get { return Input.GetKeyDown(KeyCode.Mouse1); } }
    bool CurMouseIsPlayerUnit { get { return hoveredUnit && hoveredUnit.flag.allianceId == 0; } }
    bool CurMouseIsEnemyUnit { get { return hoveredUnit && hoveredUnit.flag.allianceId != 0; } }
    bool CurMouseIsWalkable { get { return hoveredSlot && hoveredSlot.Walkable; } }
    bool CurMouseIsHostile { get { return hoveredUnit && selectedPlayerUnit && hoveredUnit.flag.allianceId != selectedPlayerUnit.flag.allianceId; } }
    bool CurMouseIsDifferentUnit { get { return hoveredUnit != selectedPlayerUnit; } }
    bool MoveCommandExecuted { get { return Mouse2Press && CurMouseIsWalkable; } }
    bool AttackCommandExecuted { get { return MousePress; } }
    bool MouseWheelRotate { get { return Input.GetAxis("Mouse ScrollWheel") != 0; } }

    
    public override IEnumerator FlagUpdate() {
        m = this;
        NullifyUnits();

        while (true) {
            if (Input.GetKeyDown(KeyCode.Return)) break;

            WaitMouseOverGrid();

            if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                mouseDirection = (4 + (mouseDirection + (int)f)) % 4;
            }
                        if (hoveredSlot ) {
                WaitUnitSelection();
                UpdateColorsDependinOnHoveringTarget();
            }

            // Set default action
            if (selectedPlayerUnit) {
                if (selectedPlayerUnit.HasActions && activeAbility == null) {
                    activeAbility = selectedPlayerUnit.abilities.move2;
                }
            }

            // load avaliable filter for current ability
            if ( selectedPlayerUnit) {
                if ((selectedPlayerUnit.abilities.newVersion && activeAbility != null) || activeAbility != null) {
                    curFilter = GridAccess.LoadAttackLayer(selectedPlayerUnit.curSlot, activeAbility.AttackMask, mouseDirection);
                }
            }
            if (Input.GetMouseButtonDown(1)) {
                selectedSlot = SelectionManager.GetMouseAsSlot2D();
            }
            // show abilities
            if (selectedUnit) {
                ShowArea(selectedUnit, curFilter);
                //UIManager.PlayerSelectAllyUnitUi(false, selectedPlayerUnit);
                // WaitAbilitySelection(); automatic on buttons
            }
            if (selectedSlot != null && selectedPlayerUnit!= null && Input.GetMouseButtonDown(1)) {
                if (selectedPlayerUnit.abilities.newVersion) {
                    if (activeAbility.actionCost > selectedPlayerUnit.ActionsLeft) {
                        Debug.Log("NOT enough actions. Can't attack.");
                    }
                    if (activeAbility.actionCost <= selectedPlayerUnit.ActionsLeft
                        && selectedPlayerUnit.CanAttackSlot(hoveredSlot, curFilter)) { // unit = enemy unit
                        bool isCurMouseHostile = hoveredSlot.filledBy && hoveredSlot.filledBy.flag.allianceId != selectedPlayerUnit.flag.allianceId;
                            // handle weapon aim
                        Debug.Log("Attacking v2 (0)");
                        ResetColorForUnit(selectedPlayerUnit, curFilter);
                        selectedPlayerUnit.AttackAction2(hoveredSlot, hoveredUnit, activeAbility);
                        CombatManager.OnUnitExecutesAction(selectedPlayerUnit);
                        yield return null;
                    }
                } else {
                    /*if (activeAbility.actionCost > selectedPlayerUnit.ActionsLeft) {
                        Debug.Log("NOT enough actions. Can't attack.");
                    }
                    if (activeAbility.actionCost <= selectedPlayerUnit.ActionsLeft
                        && selectedPlayerUnit.CanAttackSlot(hoveredSlot, curFilter)) { // unit = enemy unit
                        bool isCurMouseHostile = hoveredSlot.filledBy && hoveredSlot.filledBy.flag.allianceId != selectedPlayerUnit.flag.allianceId;
                        if (activeAbility.requiresUnit && isCurMouseHostile) {
                            // handle weapon aim
                            bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                            if (selectedPlayerUnit.equippedWeapon && selectedPlayerUnit.equippedWeapon.conePref) {
                                yield return selectedPlayerUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(selectedPlayerUnit, hoveredUnit, selectedPlayerUnit.equippedWeapon.conePref, selectedPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range));
                                aimSuccesful = CheckIfEnemyHit(hoveredUnit);
                            }
                            if (aimSuccesful) {
                                Debug.Log("Attacking (1)");
                                ResetColorForUnit(selectedPlayerUnit, curFilter);
                                selectedPlayerUnit.AttackAction(hoveredSlot, hoveredUnit, activeAbility);
                                if (selectedPlayerUnit.equippedWeapon) {
                                    selectedPlayerUnit.equippedWeapon.OnDamageEnhanceEffect(selectedPlayerUnit, hoveredSlot, hoveredUnit, activeAbility);
                                }
                                CombatManager.OnUnitExecutesAction(selectedPlayerUnit);
                            }
                        } else { //if (!curAttack.requiresUnit) { // env attack || not hostile, not env = ally
                            Debug.Log("Attacking (2)");
                            ResetColorForUnit(selectedPlayerUnit, curFilter);
                            selectedPlayerUnit.AttackAction(hoveredSlot, hoveredUnit, activeAbility);
                            CombatManager.OnUnitExecutesAction(selectedPlayerUnit);
                        }
                        yield return null;
                    }*/
                }
            }

            // map decolor when unit run out of actions.
            if (selectedUnit && selectedUnit.NoActions) {
                DeselectUnit();
                ShowUI();
            }

            if (MissionManager.levelCompleted) {
                break;
            }
            
            yield return null;

        }
        // Wait until all actions are complete
        for (int i = 0; i < units.Count; i++) {
            units[i].curSlot.RecolorSlot(0);
            while (units[i].moving) {
                yield return null;
            }
            while (units[i].attacking) {
                yield return null;
            }
        }
        yield return null;
    }

    private void ShowUI() {
        UIManager.PlayerStandardUi(!selectedPlayerUnit && !selectedUnit);
        UIManager.PlayerSelectAllyUnitUi(selectedPlayerUnit!= null && selectedUnit!= null, selectedPlayerUnit);
    }

    private void ShowArea(Unit unit, GridMask mask) {
        if (selectedPlayerUnit) {
            unit.curSlot.RecolorSlot(3);

            bool moveVersion = activeAbilityId == 0;
            RemaskActiveFilter(moveVersion ? 1 : 2, mask);
        } else {
            if (selectedUnit) // an enemy
                unit.curSlot.RecolorSlot(2);
        }
    }

    private void WaitUnitSelection() {
        hoveredUnit = SelectionManager.GetUnitUnderMouse(hoveredSlot);
        if (Input.GetMouseButtonDown(0) && hoveredUnit && (selectedPlayerUnit == null || hoveredUnit != selectedPlayerUnit)) {
            DeselectUnit();
            selectedUnit = hoveredUnit;
            if (hoveredUnit.IsPlayer)
                selectedPlayerUnit = hoveredUnit;
            activeAbility = null;
            activeAbilityId = 0;
            ShowUI();
        }
    }

    private void WaitMouseOverGrid() {
        hoveredSlot = SelectionManager.GetMouseAsSlot2D();
    }

    private void UpdateVisibleArsenal() {
        // show active clickable area for movement and attack
        Unit.activeUnit = null;
        if (selectedPlayerUnit && selectedPlayerUnit.HasActions && !selectedPlayerUnit.moving && !selectedPlayerUnit.attacking) {
            Unit.activeUnit = selectedPlayerUnit;

            ShowAttackAndMoveArea(selectedPlayerUnit, activeAbility);
        }
    }

    private void UpdateColorsDependinOnHoveringTarget() {
        // decolor last hovered unit
        if (lastHoveredUnit && lastHoveredUnit != hoveredUnit && lastHoveredUnit != selectedPlayerUnit) {
            lastHoveredUnit.curSlot.RecolorSlot(0);
        }
        lastHoveredUnit = hoveredUnit;
        // Color currently hovered unit depending on alliance
        if (hoveredUnit && hoveredUnit != selectedPlayerUnit){
            if (hoveredUnit.flag.allianceId == 0) { // player, can select
                hoveredUnit.curSlot.RecolorSlot(3);
            } else if (hoveredUnit.flag.allianceId != 0) { // enemy, maybe can attack
                hoveredUnit.curSlot.RecolorSlot(2);
            }
            //ShowArsenal
        }
    }

    private void ShowAttackAndMoveArea(Unit unit, AttackData2 attack) {
        if (!unit)
            return;
        activeAbility = attack;
        unit.curSlot.RecolorSlot(3);
        GridMask mask;
        mask = GridAccess.LoadMaskByInteractionType(selectedPlayerUnit.curSlot, unit.pathing.moveMask, 0, "Normal"); ;// unit.pathing.moveMask;
        RemaskActiveFilter(1, mask);
        mask = GridAccess.LoadAttackLayer(selectedPlayerUnit.curSlot, attack.AttackMask, mouseDirection);
        RemaskActiveFilter(2, mask);

        if (/*(attack as AoeMaskAttack)!= null*/ attack != null && hoveredSlot) {
            mask = GridAccess.LoadAttackLayer(hoveredSlot, attack.AttackMask, mouseDirection);
            RemaskActiveFilter(4, mask);
        }
    }


    private void ResetColorForUnit(Unit unit, GridMask mask) {
        if (mask)
            RemaskActiveFilter(0, mask);
        if (unit)
        unit.curSlot.RecolorSlot(0);
    }


    internal void SetActiveAbility(Unit unitSource, int atkId) {
        activeAbilityId = atkId;
        if (activeAbility != null) {
            ResetColorForUnit(unitSource, activeAbility.AttackMask);
        }
        activeAbility = unitSource.abilities.GetNormalAbilities()[atkId] as AttackData2;
        activeAbilityId = 0;
        Debug.Log("Setting active ability "+activeAbilityId);
    }
    
    private bool CheckIfEnemyHit(Unit enemy) {
        bool aimSuccesful = false;
        Vector2 fireDir = WeaponFireMode.activeUnitAimDirection;
        RaycastHit2D[] hits = Physics2D.RaycastAll(selectedPlayerUnit.transform.position, fireDir, selectedPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range);
        //Debug.Log(fireDir + " "+hits.Length);
        for (int i = 0; i < hits.Length; i++) {
            //Debug.Log(hits[i].transform.root + " "+ unit, hits[i].transform.root);
            if (hits[i].transform.root == enemy.transform) {
                aimSuccesful = true;
                break;
            }
        }
        return aimSuccesful;
    }
    
    void RemaskActiveFilter(int color, GridMask mask) {
        if (selectedUnit!=null && mask) {
            GridManager.RecolorMask(selectedUnit.curSlot, color, mask);
        }
    }

    private void DeselectUnit() {
        if (selectedUnit) {
            ResetColorForUnit(selectedUnit, curFilter);
            selectedUnit = null;
            selectedPlayerUnit = null;
            activeAbility = null;
            activeAbilityId = 0;
        }
    }

    private bool NoActionsLeft() {
        for (int i = 0; i < units.Count; i++) {
            if (!units[i].NoActions) {
                return false;
            }
        }
        return true;
    }
}

