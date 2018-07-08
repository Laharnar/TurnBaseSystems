using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UnitAnimations : MonoBehaviour {

    public string[] triggers = new string[] { "Attack" };

    Animator anim;

    private void Start() {
        anim = GetComponent<Animator>();
    }

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
