using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimations : MonoBehaviour {

    public string[] triggers = new string[] { "Attack" };

    public Animator anim;

    public void SetTrigger(int code) {
        if (code < triggers.Length && anim)
            anim.SetTrigger(triggers[code]);
    }

    internal void SetBool(string v, bool value) {
        if (anim)
            anim.SetBool(v, value);
    }

    internal int TriggerToId(string animTrigger) {
        for (int i = 0; i < triggers.Length; i++) {
            if (triggers[i] == animTrigger) {
                return i;
            }
        }
        return -1;
    }
}
