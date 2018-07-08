using System;
using System.Collections;
using UnityEngine;
[System.Serializable]
public class Pathing {
    public float speed = 10f;
    public GridMask moveMask;

    public void GoToCoroutine(Unit t, int x, int y) {
        Vector3 targetPos = GridAccess.GetItem(x, y).transform.position;
        t.curSlot.filledBy = null;
        GridAccess.GetItem(x, y).filledBy = t;
        t.curSlot = GridAccess.GetItem(x, y);
        t.StartCoroutine(GoTo(t, targetPos));
    }

    public void GoToCoroutine(Unit t, Vector3 targetPos) {
        t.curSlot.filledBy = null;
        t.StartCoroutine(GoTo(t, targetPos));
    }

    internal IEnumerator GoTo(Unit t, Vector3 targetPos) {
        t.moving = true;
        t.SetAnimBool(true);
        while (Vector3.Distance(t.transform.position, targetPos) > Time.deltaTime*speed) {
            t.transform.Translate((targetPos - t.transform.position).normalized * speed * Time.deltaTime);
            yield return null;
        }
        t.transform.position = targetPos;
        t.SetAnimBool(false);
        t.moving = false;
    }
}
