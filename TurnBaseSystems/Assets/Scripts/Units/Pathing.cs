using System;
using System.Collections;
using UnityEngine;
[System.Serializable]
public class Pathing {
    public float speed = 10f;

    public void GoToCoroutine(Unit t, int x, int y, GridManager grids) {
        Vector3 targetPos = grids.gridSlots.GetItem(x, y).transform.position;
        t.curSlot.filledBy = null;
        grids.gridSlots.GetItem(x, y).filledBy = t;
        t.curSlot = grids.gridSlots.GetItem(x, y);
        t.StartCoroutine(GoTo(t, targetPos, grids));
    }

    public void GoToCoroutine(Unit t, Vector3 targetPos, GridManager grids) {
        t.curSlot.filledBy = null;
        t.StartCoroutine(GoTo(t, targetPos, grids));
    }

    internal IEnumerator GoTo(Unit t, Vector3 targetPos, GridManager m) {
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
