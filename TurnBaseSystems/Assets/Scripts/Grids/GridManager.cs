using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager m { get; private set; }

    public Grid<GridItem> gridSlots;

    public int width, length;
    public Vector2 itemDimensions;
    public Transform pref;
    public Transform gridParent;
    public Transform rootLoader;

    private void Awake() {
        m = this;

        gridSlots = new Grid<GridItem>(width, length, rootLoader);
    }

    [ContextMenu("Update grid")]
    public void UpdateGrid () {
        if (gridSlots == null)
            gridSlots = new Grid<GridItem>(width, length);
        gridSlots.InitGrid(gridParent.transform.position, itemDimensions, pref, gridParent);
    }
}
