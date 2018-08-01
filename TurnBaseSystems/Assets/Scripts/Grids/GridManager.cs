﻿using System;
using System.Linq;
using UnityEngine;

/// <summary>
/// Generates grid items.
/// </summary>
public class GridManager : MonoBehaviour {
    public static GridManager m { get; private set; }

    public Vector2 itemDimensions;
    public Transform pref;
    public Transform gridParent;
    public Transform rootLoader;
    public Color defaultColor;
    public GridMask[] maskTemplates;

    internal static Grid NewGridInstance(Vector3 position, GridMask curAoeFilter) {
        position = SnapPoint(position, true);
        Grid g = new Grid(curAoeFilter).InitGridCenter(position, curAoeFilter);
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

    public static Vector3 SnapPoint(Vector3 point){
        return SnapPoint(point, true);
    }

    internal static Transform NewGridPrefInstance(Vector3 pos) {
        return Instantiate(m.pref, pos, new Quaternion());
    }

    internal static Vector3 SnapPoint(Vector2 point, bool offset) {
        //point = point /*- m.gridParent.transform.position*/ + (Vector3)m.itemDimensions / 2;
        if (offset && point.x < 0) {
            point.x -= m.itemDimensions.x / 2;
        } else if (offset && point.x > 0) {
            point.x += m.itemDimensions.x / 2;
        }
        if (offset && point.y < 0) {
            point.y -= m.itemDimensions.y / 2;
        } else if (offset && point.y > 0) {
            point.y += m.itemDimensions.y / 2;
        }

        point.x = point.x - point.x % m.itemDimensions.x;
        point.y = point.y - point.y % m.itemDimensions.y;
        
        return point;
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
