using System;
using UnityEngine;
public class GridItem : MonoBehaviour {
    public int gridX;
    public int gridY;

    public Unit filledBy;
    public Structure fillAsStructure;

    public bool Walkable { get { return fillAsStructure == null && filledBy == null; } }

    Color defaultColor;

    private void Awake() {
        defaultColor = transform.GetComponentInChildren<SpriteRenderer>().color;
        fillAsStructure = SelectionManager.GetAsStructure2D(transform.position);
    }

   

    internal void InitGrid(int i, int j) {
        gridX = i;
        gridY = j;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="code">0: normal, 1: selected, 2: attackable, 3: ally</param>
    internal void RecolorSlot(int code) {
        transform.GetComponentInChildren<SpriteRenderer>().color = 
            code == 0 ? defaultColor : 
            code == 1 ? Color.green :
            code == 2 ? Color.red : 
            code == 3 ? Color.blue:
            new Color(1,0.2f,0,1);
    }

    public void Null() {
        GameObject.Destroy(gameObject);
    }
}
