 using System;
using System.Collections.Generic;
using UnityEngine;
public static class SelectionManager {


    internal static Vector2 GetMouseAsPoint() {
        Vector2 selection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = GetAllSelection2D(selection);
        if (hits != null) {
            return hits[0].point;
        }
        return new Vector2();
    }

    internal static Transform GetMouseAsObject() {
        Vector2 selection = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = GetAllSelection2D(selection);
        if (hits != null) {
            return hits[0].transform;
        }
        return null;
    }

    internal static GridItem GetMouseAsSlot2D() {
        return GetAsSlot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }
    

    public static Transform GetMouseSelection2D() {
        return GetSelection2D(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public static Transform GetSelection2D(Vector2 pos) {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0);
        if (hit) {
            return hit.transform;
        }
        return null;
    }

    public static RaycastHit2D[] GetAllSelectionInDir(Vector2 pos, Vector2 dir, float dist) {
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, dir, dist);
        if (hit.Length > 0) {
            return hit;
        }
        return null;
    }

    public static RaycastHit2D[] GetAllSelection2D(Vector2 pos) {
        RaycastHit2D[] hit = Physics2D.RaycastAll(pos, Vector2.zero, 0);
        if (hit.Length > 0) {
            return hit;
        }
        return null;
    }

    public static Unit GetMouseAsUnit2D() {
        return GetAsUnit2D(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public static Unit GetAsUnit2D(Vector2 pos) {
        RaycastHit2D[] hits = GetAllSelection2D(pos);
        if (hits != null) {
            foreach (var item in hits) {
                /*Unit u = GridAccess.GetUnitAtPos(item.point);
                if (u) {
                    return u;
                }*/
                
                if (item.transform.parent == null)
                    continue;
                Unit asUnit = item.transform.parent.GetComponent<Unit>();
                if (asUnit) { 
                    return asUnit;
                }
            }
        }
        return null;
    }

    [System.Obsolete("no more prefs that use grid item compoentn")]
    public static GridItem GetAsSlot(Vector2 selection) {
        RaycastHit2D[] hits = GetAllSelection2D(selection);
        if (hits != null) {
            foreach (var item in hits) {
                GridItem slot = item.transform.GetComponent<GridItem>();
                if (slot!=null) {
                    return slot;
                }
            }
        }
        return null;
    }
    internal static Vector3 MouseAsPos() {
        return GridManager.SnapPoint(SelectionManager.GetMouseAsPoint());
    }

    public static Unit GetUnitUnderMouse(Vector3 refSlot) {
        Unit cur = GridAccess.GetUnitAtPos(refSlot);
        if (cur == null) // maybe hovering over unit's head, which is in other slot.
            cur = SelectionManager.GetMouseAsUnit2D();
        return cur;
    }

    internal static Unit[] GetAllUnitsFromDirection(Vector3 snapPos, Vector3 vector3, float range) {
        RaycastHit2D[] hits = GetAllSelectionInDir(snapPos, vector3, range);
        List<Unit> units = new List<Unit>();
        if (hits != null) {
            foreach (var item in hits) {
                Unit slot = item.transform.GetComponentInParent<Unit>();
                if (slot != null) {
                    units.Add(slot);
                }
            }
        }
        Debug.Log("Raycacst :"+hits.Length + " Units: "+units.Count);
        return units.ToArray();
    }
}
