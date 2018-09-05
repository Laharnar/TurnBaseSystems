
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
                source.anim.SetBool(animSets[activateSets[i]].animBool, animSets[activateSets[i]].animBoolValue);
            } else if (animSets[activateSets[i]].animFloat != "") {
                source.anim.SetFloat(animSets[activateSets[i]].animFloat, animSets[activateSets[i]].animFloatValue);
            }
        }
    }

    public static float GetLongestTriggerAnimLength(Unit source, int[] sourceSet) {
        if (source.abilities == null || source.abilities.abilityAnimations == null)
            return 0;
        float f = 0;
        AttackAnimationInfo[] animSets = source.abilities.abilityAnimations.animSets;
        for (int i = 0; i < sourceSet.Length; i++) {
            if (sourceSet[i] == -1) {
                Debug.Log("Undefinded, but used animation", source);
                continue;
            }
            if (sourceSet[i] >= animSets.Length) {
                Debug.Log("Incomlete set, need "+ sourceSet[i]);
                continue;
            }
            if (animSets[sourceSet[i]].animTrigger != "") {
                if (f < animSets[sourceSet[i]].animLength) {
                    f = animSets[sourceSet[i]].animLength;
                }
            }
        }
        return f;
    }
}
