using UnityEngine;
public class CurrentActionData {
    public Unit sourceExecutingUnit;
    public Vector3 attackStartedAt;
    public Vector3 attackedSlot;

    public bool attackedSelf { get { return attackedSlot == attackStartedAt; } }
    public Vector3 directionOfAttack { get { return attackedSlot - attackStartedAt; } }
}

