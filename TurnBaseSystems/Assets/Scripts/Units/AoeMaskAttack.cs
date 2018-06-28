﻿using System;
[System.Serializable]
public class AoeMaskAttack : AoeAttack{
    public int damage = 1;

    public override void ApplyDamage(Unit source, GridItem attackedSlot) {
       
        GridItem[] attackArea;
        attackArea = GridManager.LoadAoeAttackLayer(source, this, PlayerFlag.m.mouseDirection, attackedSlot);

        for (int i = 0; i < attackArea.Length; i++) {
            if (attackArea[i].filledBy)
                attackArea[i].filledBy.GetDamaged(damage);
        }
    }
}