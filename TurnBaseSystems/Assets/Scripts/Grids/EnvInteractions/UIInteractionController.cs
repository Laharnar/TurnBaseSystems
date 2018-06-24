using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Shows buttons on per slot basis. 1 button per interaction, multiple per slot.
/// <seealso cref="GridItem"/>
/// </summary>
public class UIInteractionController:MonoBehaviour {

    public static UIInteractionController m;

    List<Transform> generatedUI = new List<Transform>();

    public Transform canvasParent;

    public Transform combustibleUIPref;
    public Transform pickableUIPref;

    private void Awake() {
        m = this;
    }

    public void AddUIPiece(Transform source) {
        // add item
        generatedUI.Add(source);
    }

    public void RemoveUIPiece(Transform source) {
        // add item
        generatedUI.Remove(source);
        Destroy(source);
    }

    internal static void ShowEnvInteractions(Unit playerActiveUnit) {
        // reset all ui
        ClearUI();
        // scan area
        GridItem[] items = InteractionScanner.Scan(playerActiveUnit);
        // activate all ui's of slots in area
        for (int i = 0; i < items.Length; i++) {
            OverlayUI(items[i]);
        }
    }

    private static void ClearUI() {
        for (int i = 0; i < m.generatedUI.Count; i++) {
            if (m.generatedUI[i])
            GameObject.Destroy(m.generatedUI[i].gameObject);
        }
        m.generatedUI.Clear();
    }

    private static void OverlayUI(GridItem slot) {
        InteractiveEnvirounment interaction = slot.slotInteractions;
        for (int i = 0; i < interaction.interactions.Count; i++) {
            if (interaction.interactions[i].InteractionMatch("Combustible")) {
                Transform t = Instantiate(m.combustibleUIPref, slot.transform.position+new Vector3(0,i), new Quaternion(), m.canvasParent);
                ButtonInteraction bi = t.gameObject.GetComponent<ButtonInteraction>();
                bi.interaction = interaction.interactions[i];
                bi.source = slot.fillAsStructure;
                m.AddUIPiece(t);
            }
            else if (interaction.interactions[i].InteractionMatch("Pickable")) {
                Transform t = Instantiate(m.pickableUIPref, slot.transform.position + new Vector3(0, i), new Quaternion(), m.canvasParent);
                ButtonInteraction bi = t.gameObject.GetComponent<ButtonInteraction>();
                bi.interaction = interaction.interactions[i];
                bi.weaponSource = slot.fillAsPickup;
                m.AddUIPiece(t);
            }
        }
    }

    internal static void ClearEnvInteractions() {
        ClearUI();
    }
}
