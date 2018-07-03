using System;
using System.Collections;
using UnityEngine;
public class PlayerFlag : FlagController {

    public static PlayerFlag m;

    Unit selectedPlayerUnit;

    Unit hoveredUnit;
    Unit lastHoveredUnit;
    GridItem hoveredSlot;

    Coroutine twoStepCoro;
    bool RunningTwoStepAbility { get { return twoStepCoro != null; } }

    Unit coroUnitSource;
    int coroAtkId;
    public Attack curAttack;
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
        turnDone = false;
        NullifyUnits();

        while (true) {
            if (units.Count == 0 || NoActionsLeft() || Input.GetKeyDown(KeyCode.Return)) break;

            // update offset. 
            if (selectedPlayerUnit && hoveredSlot) {
                mouseToSelectOffset.x = hoveredSlot.gridX - selectedPlayerUnit.gridX;
                mouseToSelectOffset.y = hoveredSlot.gridY - selectedPlayerUnit.gridY;
            }
            if (MouseWheelRotate) {
                float f = Input.GetAxis("Mouse ScrollWheel");
                f = f < 0 ? -1 : f > 0 ? 1 : 0;
                mouseDirection = (4+(mouseDirection+ (int)f)) % 4;
            }

            UpdateVisibleArsenal();
            UpdateColorsDependinOnHoveringTarget();

            // get hovered slot and unit in it
            hoveredSlot = SelectionManager.GetMouseAsSlot2D();
            if (hoveredSlot == null) { // didn't hover over map
                yield return null;
                continue;
            }

            hoveredUnit = GetUnitUnderMouse();

            //UpdateInteractionFilter();
            

            // *t button ability press -> a) wait for press-> execute selected attack
            // b) wait for cancel
            if (RunningTwoStepAbility) {
                if (MousePress || Input.GetKeyDown(KeyCode.Escape) || MoveCommandExecuted && selectedPlayerUnit && selectedPlayerUnit.CanMoveTo(hoveredSlot)
                    || AttackCommandExecuted && selectedPlayerUnit && selectedPlayerUnit.CanAttackSlot(hoveredSlot, curFilter)) {
                    //curAttack = coroUnitSource.abilities.GetNormalAbilities()[coroAtkId];
                    coroUnitSource.StopCoroutine(twoStepCoro);
                    twoStepCoro = null;
                }
            } else {

                // Selecting player units when pressed.
                if (MousePress && CurMouseIsPlayerUnit && CurMouseIsDifferentUnit) {
                    //selectionChanged = true;
                    DeselectUnit();

                    if (hoveredUnit.HasActions) {
                        selectedPlayerUnit = hoveredUnit;
                        PlayerUIAbilityList.LoadAbilitiesOnUI(selectedPlayerUnit);
                        curAttack = selectedPlayerUnit.abilities.BasicAttack;
                        //ShowArsenal(selectedPlayerUnit, selectedPlayerUnit.abilities.BasicAttack);
                    }
                }
            }

            // Find filter for active attack, Attack is changed by UI.
            if (curAttack != null && selectedPlayerUnit) {
                curFilter = GridManager.LoadAttackLayer(selectedPlayerUnit, curAttack, mouseDirection);
            }
            // Note: attack and move commands override the coro call.
            // move
            if (MoveCommandExecuted && selectedPlayerUnit && selectedPlayerUnit.CanMoveTo(hoveredSlot)) {
                //selectionChanged = true;
                MoveCurTo(hoveredSlot);

                yield return null;
            }
            // attack
            else {
                if (AttackCommandExecuted && selectedPlayerUnit && selectedPlayerUnit.CanAttackWith(curAttack) && selectedPlayerUnit.CanAttackSlot(hoveredSlot, curFilter)) { // unit = enemy unit
                    bool isHostileType = curAttack != null && curAttack.requiresUnit && CurMouseIsHostile;
                    Debug.Log("Passable: " + true + " requiresAndFoundEnemy:" + isHostileType + " notReqAndFoundEnv" + !curAttack.requiresUnit + " foundAlly" + !(isHostileType || !curAttack.requiresUnit));

                    if (curAttack.requiresUnit && CurMouseIsHostile) {
                        // handle weapon aim
                        bool aimSuccesful = true;// by default, always hit, if weapon doesn't use cone ability.
                        if (selectedPlayerUnit.equippedWeapon && selectedPlayerUnit.equippedWeapon.conePref) {
                            yield return selectedPlayerUnit.StartCoroutine(WeaponFireMode.WaitPlayerToSetAim(selectedPlayerUnit, hoveredUnit, selectedPlayerUnit.equippedWeapon.conePref, selectedPlayerUnit.equippedWeapon.StandardAttack.attackMask.Range));
                            aimSuccesful = CheckIfEnemyHit(hoveredUnit);
                        }

                        if (aimSuccesful) {
                            ResetColorForUnit(selectedPlayerUnit);
                            selectedPlayerUnit.AttackAction(hoveredSlot, hoveredUnit, curAttack);
                            if (selectedPlayerUnit.equippedWeapon) {
                                selectedPlayerUnit.equippedWeapon.OnDamageEnhanceEffect(selectedPlayerUnit, hoveredSlot, hoveredUnit, curAttack);
                            }
                        }
                    } else { //if (!curAttack.requiresUnit) { // env attack || not hostile, not env = ally
                        ResetColorForUnit(selectedPlayerUnit);
                        selectedPlayerUnit.AttackAction(hoveredSlot, hoveredUnit, curAttack);
                    }/* else if (curAttack.requiresUnit && !CurMouseIsHostile) { // 
                        ResetColorForUnit(selectedPlayerUnit);
                        selectedPlayerUnit.AttackAction(hoveredSlot, hoveredUnit, curAttack);
                    }*/
                    yield return null;
                }
            }
            
            // Reload env interaction buttons when player is selected.
            /*
            if (selectionChanged) {
                if (selectedPlayerUnit) {
                    UIInteractionController.ShowEnvInteractions(selectedPlayerUnit);
                } else {
                    UIInteractionController.ClearEnvInteractions();
                }
            }*/

            UIManager.PlayerStandardUi(!selectedPlayerUnit);
            UIManager.PlayerSelectAllyUnitUi(selectedPlayerUnit);

            // map decolor when unit run out of actions.
            if (selectedPlayerUnit && selectedPlayerUnit.NoActions) {
                DeselectUnit();
                UIInteractionController.ClearEnvInteractions();
            }

            yield return null;

            NullifyUnits();
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
        turnDone = true;
        yield return null;
    }

    private void UpdateVisibleArsenal() {
        // show active clickable area for movement and attack
        Unit.activeUnit = null;
        if (selectedPlayerUnit && selectedPlayerUnit.HasActions && !selectedPlayerUnit.moving && !selectedPlayerUnit.attacking) {
            Unit.activeUnit = selectedPlayerUnit;
            ShowArsenal(selectedPlayerUnit, curAttack);
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

    private void ShowArsenal(Unit unit, Attack attack) {
        if (!unit)
            return;
        curAttack = attack;
        unit.curSlot.RecolorSlot(3);
        GridMask mask;
        mask = LoadInteractionsInArea(selectedPlayerUnit.curSlot, unit.pathing.moveMask, "Normal"); ;// unit.pathing.moveMask;
        RemaskActiveFilter(1, mask);

        mask = attack.attackMask;
        if (attack.attackMask.rotateable)
            mask = GridMask.RotateMask(mask, mouseDirection);

        mask = LoadInteractionsInArea(selectedPlayerUnit.curSlot, mask, attack.attackType);
        RemaskActiveFilter(2, mask);

        if ((attack as AoeMaskAttack)!= null && hoveredSlot) {
            mask = (attack as AoeMaskAttack).aoeMask;
            if (mask.rotateable)
                mask = GridMask.RotateMask(mask, mouseDirection);
            mask = LoadInteractionsInArea(hoveredSlot, mask, attack.attackType);
            RemaskActiveFilter(4, mask);

        }
    }


    private void ResetColorForUnit(Unit unit) {
        RemaskActiveFilter(0, unit.pathing.moveMask);
        RemaskActiveFilter(0, curFilter != null ? curFilter : unit.abilities.BasicMask);
        unit.curSlot.RecolorSlot(0);
    }
    /*private void UpdateInteractionFilter() {
        if (!selectedPlayerUnit)
            return;
        if (curAttack!= null ) {
            Debug.Log("UNCOMMENT THIS");
            //curFilter = LoadInteractionsInArea(selectedPlayerUnit.curSlot, curAttack.attackMask);
        } else {
            Debug.Log("Attack is null");
        }
    }*/

    /// <summary>
    /// Activates slots in area that fit filtering parameters
    /// </summary>
    /// <param name="curAttack"></param>
    [System.Obsolete("Replaced by GridManager.LoadAttackLayer")]
    private GridMask LoadInteractionsInArea(GridItem slot, GridMask mask, string attackType) {
        GridItem[] items = GridManager.GetSlotsInMask(slot.gridX, slot.gridY, mask);
        //return GridMask.FullMask(items);
        return AiHelper.FilterByInteractions(slot, items, attackType, mask);
    }

    private Unit GetUnitUnderMouse() {
        Unit cur = hoveredSlot.filledBy;
        if (cur == null) // maybe hovering over unit's head, which is in other slot.
            cur = SelectionManager.GetMouseAsUnit2D();
        return cur;
    }

    private void MoveCurTo(GridItem item) {
        ResetColorForUnit(selectedPlayerUnit);

        selectedPlayerUnit.MoveAction(item);

    }

    internal void StartTwoStepAttack(Unit unitSource, int atkId) {
        if (twoStepCoro != null)
            unitSource.StopCoroutine(twoStepCoro);
        
        twoStepCoro = unitSource.StartCoroutine(HoldAttackForUserInput(unitSource, atkId));
    }

    IEnumerator HoldAttackForUserInput(Unit source, int atkId) {
        coroUnitSource = source;
        coroAtkId = atkId;
        
        curAttack = coroUnitSource.abilities.GetNormalAbilities()[coroAtkId];
        ResetColorForUnit(coroUnitSource);
        //ShowArsenal(coroUnitSource, coroUnitSource.abilities.GetNormalAbilities()[coroAtkId].attackMask);
        
        yield return null;
        /*while (true) {
            yield return null;
        }*/
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
        if (selectedPlayerUnit && mask) {
            GridManager.RecolorMask(selectedPlayerUnit.curSlot, color, mask);
        }
    }

    private void DeselectUnit() {
        if (selectedPlayerUnit) {
            ResetColorForUnit(selectedPlayerUnit);
            selectedPlayerUnit = null;
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

