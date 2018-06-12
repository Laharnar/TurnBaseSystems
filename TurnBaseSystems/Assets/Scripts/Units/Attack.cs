using System;

[System.Serializable]
public abstract class Attack {
    public abstract void ApplyDamage(Unit source, GridItem attackedSlot);
}
