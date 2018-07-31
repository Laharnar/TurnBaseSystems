using System;
using System.Collections;
using UnityEngine;
[System.Serializable]
public class Pathing {
    public float speed = 10f;
    //public GridMask moveMask;


    public static void SetAnimBool(Unit t, bool value, string s) {
        if (t && t.anim) {
            t.anim.SetBool(s, value);
        }
    }
    internal IEnumerator GoTo(Unit t, Vector3 targetPos, string s) {
        t.moving = true;
        //AttackData2.RunAnimations(t, t.abilities.move2.standard.animSets);
        SetAnimBool(t, true, s);
        //t.anim.SetTrigger(animSets[activateSets[i]].animTrigger);
        while (Vector3.Distance(t.transform.position, targetPos) > Time.deltaTime*speed) {
            t.transform.Translate((targetPos - t.transform.position).normalized * speed * Time.deltaTime);
            yield return null;
        }
        t.transform.position = targetPos;
        SetAnimBool(t, false, s);
        t.moving = false;
    }
}
