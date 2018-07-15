﻿ using System;
using UnityEngine;
public class SelectionManager : MonoBehaviour{

    public static SelectionManager m;

    private void Awake() {
        m = this;
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

    internal static Structure GetAsStructure2D(Vector3 position) {
        RaycastHit2D[] hits = GetAllSelection2D(position);
        if (hits != null) {
            foreach (var item in hits) {
                Structure asStructure = item.transform.GetComponent<Structure>();
                if (asStructure) {
                    return asStructure;
                }
            }
        }
        return null;
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

    public static GridItem GetAsSlot(Vector2 selection) {
        RaycastHit2D[] hits = GetAllSelection2D(selection);
        if (hits != null) {
            foreach (var item in hits) {
                GridItem slot = item.transform.GetComponent<GridItem>();
                if (slot) {
                    return slot;
                }
            }
        }
        return null;
    }

    public static Unit GetUnitUnderMouse(GridItem refSlot) {
        Unit cur = refSlot.filledBy;
        if (cur == null) // maybe hovering over unit's head, which is in other slot.
            cur = SelectionManager.GetMouseAsUnit2D();
        return cur;
    }
}