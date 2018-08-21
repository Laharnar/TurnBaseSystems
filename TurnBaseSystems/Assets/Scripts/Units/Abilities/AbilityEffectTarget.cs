using UnityEngine;
[System.Serializable]
    [System.Obsolete("Hard to setup")]
public class AbilityEffectTarget {

    [Header("Which ability type")]
    public int[] dataTypes;
    [Header("Which id within that arr.")]
    public int[] targets;

}
