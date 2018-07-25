using System;
using System.Linq;
using UnityEngine;

public partial class GridManager : MonoBehaviour {
    public static GridManager m { get; private set; }

    public Grid gridSlots;

    public int width, length;

    public Vector2 itemDimensions;
    public Transform pref;
    public Transform gridParent;
    public Transform rootLoader;
    public Color defaultColor;

    internal static Grid NewGridInstance(Vector3 position, GridMask curAoeFilter) {
        Grid g = new Grid(curAoeFilter).InitGridCenter(position);
        return g;
    }

    private void Awake() {
        m = this;
        defaultColor = pref.GetComponentInChildren<SpriteRenderer>().color;
        //gridSlots = new Grid(width, length, rootLoader);
        //UpdateGrid();
    }

    private void Start() {
        //Weapon.AssignAllDroppedWeaponsToSlots();
    }

    /*[ContextMenu("Update grid")]
    public void UpdateGrid () {
        if (gridSlots == null)
            gridSlots = new Grid(width, length);
        gridSlots.InitGrid(gridParent.transform.position, itemDimensions, pref, gridParent);
    }*/

    public static Vector3 SnapPoint(Vector3 point){
        Vector3 o = point;
        point = point - m.gridParent.transform.position + (Vector3)m.itemDimensions / 2;
        point.x = point.x - point.x % m.itemDimensions.x;
        point.y = point.y - point.y % m.itemDimensions.y;
        return point;
    }

    internal static Transform NewGridPrefInstance(Vector3 pos) {
        return Instantiate(m.pref, pos, new Quaternion());
    }

    internal static GridItem NewGridInstance(Vector3 pos) {
        GridItem it = new GridItem();
        it.instance = Instantiate(m.pref);
        it.worldPosition = pos;
        it.instance.parent = GridManager.m.gridParent;
        it.instance.name = "(" + pos.x + ":" + pos.y + ")" + it.instance.name;
        Vector2 snapped = SnapPoint(pos);
        //it.InitGrid((int)snapped.x, (int)snapped.y);
        return it;
    }

    internal static GridItem NewGridInstanceAtMouse() {
        return NewGridInstance(SelectionManager.GetMouseAsPoint());
    }
}
