using System;
using UnityEngine;
public abstract class UnitAbilities : MonoBehaviour {
    public abstract Attack BasicAttack { get; }
    public abstract GridMask BasicMask { get; }

    public abstract EnvirounmentalAttack[] GetEnvAbilities();
}
