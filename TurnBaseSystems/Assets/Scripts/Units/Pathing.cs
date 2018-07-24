using System;
using System.Collections;
using UnityEngine;
[System.Serializable]
public class Pathing {
    public float speed = 10f;
    public GridMask moveMask;

    public void GoToCoroutine(Unit t, Vector3 targetPos) {
        //Vector3 targetPos=GridAccess.GetItem(x, y).worldPosition
        //t.curSlot.filledBy = null;
        //GridAccess.GetItem(x, y).filledBy = t;
        //t.curSlot = GridAccess.GetItem(x, y);
        t.StartCoroutine(GoTo(t, targetPos));
    }

    public static void SetAnimBool(Unit t, bool value) {
        if (t && t.anim) {
            t.anim.SetBool("Walk", value);
        }
    }
    internal IEnumerator GoTo(Unit t, Vector3 targetPos) {
        t.moving = true;
        //AttackData2.RunAnimations(t, t.abilities.move2.standard.animSets);
        SetAnimBool(t, true);
        //t.anim.SetTrigger(animSets[activateSets[i]].animTrigger);
        while (Vector3.Distance(t.transform.position, targetPos) > Time.deltaTime*speed) {
            t.transform.Translate((targetPos - t.transform.position).normalized * speed * Time.deltaTime);
            yield return null;
        }
        t.transform.position = targetPos;
        SetAnimBool(t, false);
        t.moving = false;
    }
}
