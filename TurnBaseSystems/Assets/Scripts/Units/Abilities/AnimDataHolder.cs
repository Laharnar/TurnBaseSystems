
using System;
using UnityEngine;

public class AnimDataHolder:MonoBehaviour {

    public AttackAnimationInfo[] animSets;

    internal void Run(Unit source, params int[] activateSets) {
        for (int i = 0; i < activateSets.Length; i++) {
            if (activateSets[i] > animSets.Length) {
                Debug.Log("Incomplete set. required "+activateSets[i] + " found "+animSets.Length);
                continue;
            }
            if (animSets[activateSets[i]].animTrigger != "") {
                source.anim.SetTrigger(animSets[activateSets[i]].animTrigger);
            } else if (animSets[activateSets[i]].animBool != "") {
                source.anim.SetTrigger(animSets[activateSets[i]].animBool);
            }
        }
    }

    public static float GetLongestTriggerAnimLength(Unit source, int[] activateSets) {
        float f = 0;
        AttackAnimationInfo[] animSets = source.abilities.abilityAnimations.animSets;
        for (int i = 0; i < animSets.Length; i++) {
            if (animSets[activateSets[i]].animTrigger != "") {
                if (f < animSets[activateSets[i]].animLength) {
                    f = animSets[activateSets[i]].animLength;
                }
            }
        }
        return f;
    }
}
