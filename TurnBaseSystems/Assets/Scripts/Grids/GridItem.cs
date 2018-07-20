using System;
using System.Collections.Generic;
using UnityEngine;

public class GridItem : MonoBehaviour {
    public int gridX;
    public int gridY;

    public Unit filledBy;
    public Structure fillAsStructure;
    public Weapon fillAsPickup;
    public int AP = 3;
    //public LocationMaterial material;
    Color defaultColor;

    /// <summary>
    /// Abilities are loaded on level load or later, from all objects that can be found on this slot.
    /// Note: this is on child object. Don't use it's position as reference.
    /// </summary>
    public InteractiveEnvirounment avaliableInteractions;

    public bool Walkable { get { return fillAsStructure == null && filledBy == null; } }

    internal bool TryDrainGround() {
        if (AP > 0) {
            AP--;
            return true;
        }
        return false;
    }

    public void RestoreFlora() {
        AP++;
    }

    private void Awake() {
        defaultColor = transform.GetComponentInChildren<SpriteRenderer>().color;

        InitTempChildAsDataHolder();

        // Attach interactions from structures on this slot.
        fillAsStructure = SelectionManager.GetAsStructure2D(transform.position);
        if (fillAsStructure) {
            avaliableInteractions.interactions.AddRange(fillAsStructure.GetComponent<InteractiveEnvirounment>().Copies());
        }
    }

    private void InitTempChildAsDataHolder() {
        GameObject go = new GameObject("InteractionsTemp");
        go.transform.parent = transform;
        avaliableInteractions = go.AddComponent<InteractiveEnvirounment>();
        avaliableInteractions.transform.position = transform.position;
    }

    internal void InitGrid(int i, int j) {
        gridX = i;
        gridY = j;
    }


    internal void RemoveInteractions(List<Interaction> interactions) {
        for (int i = 0; i < interactions.Count; i++) {
            avaliableInteractions.RemoveByType(interactions[i].interactionType);
        }
    }

    internal void AddEnvInteraction(params string[] v) {
        for (int i = 0; i < v.Length; i++) {
            EnvInteraction eint = ScriptableObject.CreateInstance<EnvInteraction>();
            eint.interactionType = v[i];
            avaliableInteractions.interactions.Add(eint);
        }
    }

    internal static bool TypeFilter(GridItem gridItem, string attackType) {
        if (attackType == "Normal") {
            return gridItem.avaliableInteractions.interactions.Count == 0 || gridItem.avaliableInteractions.interactions[0].InteractionMatch("Pickable");
        }
        for (int i = 0; i < gridItem.avaliableInteractions.interactions.Count; i++) {
            if (gridItem.avaliableInteractions.interactions[i].InteractionMatch(attackType)) {
                Debug.Log("Found match with "+attackType);
                return true;
            }
        }
        Debug.Log("No match with "+attackType);
        return false;
    }


    /// <summary>
    /// 0: normal, 1: selected green, 2: attackable red, 3: ally blue, 4: orange, 5: yellow
    /// </summary>
    /// <param name="code">0: normal, 1: selected green, 2: attackable red, 3: ally blue, 4: orange, 5: yellow</param>
    internal void RecolorSlot(int code) {
        transform.GetComponentInChildren<SpriteRenderer>().color = 
            code == 0 ? defaultColor : 
            code == 1 ? Color.green :
            code == 2 ? Color.red : 
            code == 3 ? Color.blue :
            code == 4 ? new Color(1,0.2f,0,1)://orange
                Color.yellow;
    }

    public void Null() {
        GameObject.Destroy(gameObject);
    }


    internal void DetachPickupFromSlot() {
        Debug.LogWarning("Warning:Sketchy code. Clears ALL interactions from temp object, not only pickup.");
        avaliableInteractions.interactions.Clear();
        fillAsPickup = null;
    }
}
