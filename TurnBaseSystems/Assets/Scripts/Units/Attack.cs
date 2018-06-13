using System;

[System.Serializable]
public abstract class Attack {
    public GridMask attackMask;

    public abstract void ApplyDamage(Unit source, GridItem attackedSlot);
}
