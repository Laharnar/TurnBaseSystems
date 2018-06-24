using System;
using UnityEngine;

public class GridItem : MonoBehaviour {
    public int gridX;
    public int gridY;

    public Unit filledBy;
    public Structure fillAsStructure;
    public Weapon fillAsPickup;

    //public LocationMaterial material;

    /// <summary>
    /// Abilities are loaded on level load or later, from all objects that can be found on this slot.
    /// Note: this is on child object. Don't use it's position as reference.
    /// </summary>
    public InteractiveEnvirounment slotInteractions;

    public bool Walkable { get { return fillAsStructure == null && filledBy == null; } }
    Color defaultColor;

    private void Awake() {
        defaultColor = transform.GetComponentInChildren<SpriteRenderer>().color;

        // Add empty child holder for interactions
        GameObject go = new GameObject("InteractionsTemp");
        go.transform.parent = transform;
        slotInteractions = InteractiveEnvirounment.AttachScript(go);
        slotInteractions.transform.position = transform.position;

        // Attach interactions from structures on this slot.
        fillAsStructure = SelectionManager.GetAsStructure2D(transform.position);
        if (fillAsStructure) {
            slotInteractions.interactions.AddRange(fillAsStructure.GetComponent<InteractiveEnvirounment>().Copies());
        }
    }

    internal void InitGrid(int i, int j) {
        gridX = i;
        gridY = j;
    }

    internal static bool TypeFilter(GridItem gridItem, string attackType) {
        if (attackType == "Normal") {
            return gridItem.slotInteractions.interactions.Count == 0 || gridItem.slotInteractions.interactions[0].GetType() == typeof(Pickable);
        }
        for (int i = 0; i < gridItem.slotInteractions.interactions.Count; i++) {
            if (gridItem.slotInteractions.interactions[i].GetType()
                .Name == attackType) {
                return true;
            }
        }
        return false;
    }

    internal void CreateWall() {
        throw new NotImplementedException();
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
            new Color(1,0.2f,0,1);//orange
    }

    public void Null() {
        GameObject.Destroy(gameObject);
    }


    internal void DetachPickupFromSlot() {
        Debug.LogWarning("Warning:Sketchy code. Clears ALL interactions from temp object, not only pickup.");
        slotInteractions.interactions.Clear();
        fillAsPickup = null;
    }
}
