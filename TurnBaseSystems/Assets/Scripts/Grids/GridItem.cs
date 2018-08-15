using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class GridItem {
    /// <summary>
    /// Accessing the position inside the certain grid
    /// </summary>
    //public Vector2 gridPosition;

    public Unit filledBy;
    Color defaultColor;
    public Transform instance;

    // snapped position of object inside the grid
    Vector3 _pos;
    internal int gridX;
    internal int gridY;

    internal Vector3 worldPosition {
        get {
            return _pos;
        }
        set {
            _pos = value;
            if (instance != null) {
                instance.position = _pos;
            }
        }
    }

    /*internal bool TryDrainGround() {
        if (AP > 0) {
            AP--;
            return true;
        }
        return false;
    }*/

    /*public void RestoreFlora() {
        AP++;
    }*/

    /*private void Awake() {
        //defaultColor = transform.GetComponentInChildren<SpriteRenderer>().color;

        //InitTempChildAsDataHolder();

        // Attach interactions from structures on this slot.
        /*fillAsStructure = SelectionManager.GetAsStructure2D(transform.position);
        if (fillAsStructure) {
            avaliableInteractions.interactions.AddRange(fillAsStructure.GetComponent<InteractiveEnvirounment>().Copies());
        }*/
    //}

   /* private void InitTempChildAsDataHolder() {
        /*GameObject go = new GameObject("InteractionsTemp");
        go.transform.parent = transform;
        avaliableInteractions = go.AddComponent<InteractiveEnvirounment>();
        avaliableInteractions.transform.position = transform.position;*/
    //}
    /*
    internal void InitGrid(int i, int j) {
        gridPosition.x = i;
        position.y = j;
    }*/

    /*internal void RemoveInteractions(List<Interaction> interactions) {
        for (int i = 0; i < interactions.Count; i++) {
            avaliableInteractions.RemoveByType(interactions[i].interactionType);
        }
    }*/
    /*
    [System.Obsolete("Shouldnt be used")]
    internal void AddEnvInteraction(params string[] v) {
        for (int i = 0; i < v.Length; i++) {
            EnvInteraction eint = ScriptableObject.CreateInstance<EnvInteraction>();
            eint.interactionType = v[i];
            avaliableInteractions.interactions.Add(eint);
        }
    }
    */


    /// <summary>
    /// 0: normal, 1: selected green, 2: attackable red, 3: ally blue, 4: orange, 5: yellow
    /// </summary>
    /// <param name="code">0: normal, 1: selected green, 2: attackable red, 3: ally blue, 4: orange, 5: yellow</param>
    internal void RecolorSlot(int code) {
        if (!instance)
            instance = GridManager.NewGridPrefInstance(worldPosition);
        instance.GetComponentInChildren<SpriteRenderer>().color = 
            code == 0 ? GridManager.m.defaultColor : 
            code == 1 ? Color.green :
            code == 2 ? Color.red : 
            code == 3 ? Color.blue :
            code == 4 ? new Color(1,0.6f,0,2)://orange
                Color.yellow;
    }

    /*
    internal void DetachPickupFromSlot() {
        Debug.LogWarning("Warning:Sketchy code. Clears ALL interactions from temp object, not only pickup.");
        avaliableInteractions.interactions.Clear();
        fillAsPickup = null;
    }*/
}
