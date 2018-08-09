using UnityEngine;
public class CI {
    public static AttackData2 activeAbility;
    public static CombatEventMask curActivator;
    public static BUFFAttackData activeOrigBuff;
    public static BuffUnitData activeBuffData;

    public static Unit sourceExecutingUnit;
    public static Vector3 attackStartedAt;
    public static Vector3 attackedSlot;
    internal static Unit sourceSecondaryExecUnit;

    public static Vector3 directionOfAttack { get { return attackedSlot - attackStartedAt; } }
}
