using UnityEngine;
[System.Serializable]
public class AbilityEffectTarget {

    [Header("Which ability type")]
    public int[] dataTypes;
    [Header("Which id within that arr.")]
    public int[] targets;

    public void Execute(AttackDataLib atkLib, CombatEventMask curActivator) {
        for (int i = 0; i < targets.Length; i++) {
            int k = targets[i];

            AbilityEffect[] lib = atkLib.GetLib();
            if (k < dataTypes.Length || dataTypes[k] < lib.Length) {
                if (lib[dataTypes[targets[i]]].used)
                    lib[k].AtkBehaviourExecute();
            } else {
                Debug.Log("missing types in lib "+ dataTypes[targets[i]] + " "+targets[i]);
            }
        }
    }
}
