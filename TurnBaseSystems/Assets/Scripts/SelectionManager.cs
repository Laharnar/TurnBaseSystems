using System;
using UnityEngine;
public class SelectionManager : MonoBehaviour{

    public static SelectionManager m;

    public Unit selectedUnit;

    private void Awake() {
        m = this;
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Mouse0))
            selectedUnit = GetMouseAsUnit2D();
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
}
