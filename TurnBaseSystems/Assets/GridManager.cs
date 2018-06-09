using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GridManager : MonoBehaviour {
    public static GridManager m { get; private set; }

    public Grid<SceneTransformData> gridSlots;
    public List<Unit> unitGridData = new List<Unit>();

    public int width, length;
    public Vector2 itemDimensions;
    public Transform pref;
    public Transform gridParent;

    private void Start() {
        m = this;
    }

    [ContextMenu("Update grid")]
    public void UpdateGrid () {
        if (gridSlots == null)
            gridSlots = new Grid<SceneTransformData>(width, length);
        gridSlots.InitGrid(gridParent.transform.position, itemDimensions, pref, gridParent);
    }
}
