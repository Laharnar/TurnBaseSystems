﻿using System;
using System.Collections;
using UnityEngine;

public class Unit :MonoBehaviour, ISlotItem{
    public int gridX, gridY;
    public bool moving = false;
    public Pathing pathing;
    public Alliance flag;

    public Animator anim;
    public bool NoActions { get { return actionsLeft <= 0; } }

    public bool CanMove { get { return actionsLeft >=1; } }
    public bool CanAttack { get { return actionsLeft >= 1; } }

    public int hp = 5;

    public int maxActions = 2;
    int actionsLeft = 2;

    bool dead = false;

    public UnitAbilities abilities;
    public AiLogic ai;

    public GridItem curSlot;

    private void Start() {
        ResetActions();
        GridItem slot = SelectionManager.GetAsSlot(transform.position-Vector3.forward);
        curSlot = slot;
        Move(slot);
        FlagManager.RegisterUnit(this);
    }
    public void ResetActions(int val=-1) {
        if (val == -1)
            actionsLeft = maxActions;
        else actionsLeft = val;
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
        actionsLeft-=2;
        atk.ApplyDamage(this, attackedSlot);
    }

    public void GetDamaged(int v) {
        if (dead) return;
        hp -= v;
        if (hp <= 0) {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death() {
        dead = true;
        yield return null;
        Destroy(gameObject);
    }

    public void Move(Vector2 pos) {
        if (moving) return;
        pathing.GoToCoroutine(this, pos, GridManager.m);
    }
}