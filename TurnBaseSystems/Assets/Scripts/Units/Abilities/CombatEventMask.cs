using System;

[System.Serializable]
public class CombatEventMask {
    public bool
        never,
        //onAnyBuffTick,
        //onAllyBuffTick,
        //onHostileBuffTick,
        onAnyTurnStart,
        onAnyTurnEnd,
        onEnemyTurnStart,
        onEnemyTurnEnd,
        onPlayerTurnStart,
        onPlayerTurnEnd,
        onDamaged,
        onMove,
        onAttack,
        onStepOnEnemy,
        onEnemyEnterRange,
        onAllyEnterRange,
        onEnemyExitRange,
        onAllyExitRange,
        onCooldownTick, 
        onUnitDies;

    public bool[] ToArray() {
        return new bool[]{
            never,
        //onAnyBuffTick,
        //onAllyBuffTick,
        //onHostileBuffTick,
        onAnyTurnStart,
        onAnyTurnEnd,
        onEnemyTurnStart,
        onEnemyTurnEnd,
        onPlayerTurnStart,
        onPlayerTurnEnd,
        onDamaged,
        onMove,
        onAttack,
        onStepOnEnemy,
        onEnemyEnterRange,
        onAllyEnterRange,
        onEnemyExitRange,
        onAllyExitRange,
        onCooldownTick,
        onUnitDies
        };
    }

    public static bool CanActivate(CombatEventMask combatStateActivator, CombatEventMask abilityActivator) {
        bool[] a1 = combatStateActivator.ToArray();
        bool[] a2 = abilityActivator.ToArray();
        if (a1[0] == true || a2[0] == true) // handle never
            return false;
        for (int i = 1  ; i < a1.Length; i++) { // skip never
            if (a1[i] && a1[i] == a2[i]) {
                return true;
            }
        }
        return false;
    }

    internal void Reset() {
        //onAnyBuffTick=
        //onAllyBuffTick=
        //onHostileBuffTick=
        onAnyTurnStart =
        onAnyTurnEnd =
        onEnemyTurnStart =
        onEnemyTurnEnd =
        onPlayerTurnStart =
        onPlayerTurnEnd =
        onDamaged = 
        onMove = 
        onAttack = 
        onStepOnEnemy = 
        onEnemyEnterRange = 
        onAllyEnterRange = 
        onEnemyExitRange = 
        onAllyExitRange = 
        onCooldownTick =
        onUnitDies = false;
    }
}