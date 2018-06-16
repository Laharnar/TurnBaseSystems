using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractionController:MonoBehaviour {

    public static UIInteractionController m;
    List<Transform> activeUI = new List<Transform>();

    public Transform canvasParent;

    public Transform combustibleUIPref;
    private void Awake() {
        m = this;
    }

    public void AddInteraction(Transform source) {
        // add item
        activeUI.Add(source);
    }

    public void RemoveInteraction(Transform source) {
        // add item
        activeUI.Remove(source);
        Destroy(source);
    }

    public static InteractibleAsAbility GetInteractions(GridItem slot) {
        return slot.avaliableAbilities;
    }

    internal static void ShowInteractions(Unit playerActiveUnit) {
        // reset all ui
        ClearUI();
        // scan area
        GridItem[] items = GridManager.GetSlotsInInteractiveRange(playerActiveUnit, null);
        // activate all ui's of slots in area
        for (int i = 0; i < items.Length; i++) {
            OverlayUI(items[i]);
        }
    }

    private static void ClearUI() {
        for (int i = 0; i < m.activeUI.Count; i++) {
            if (m.activeUI[i])
            GameObject.Destroy(m.activeUI[i].gameObject);
        }
        m.activeUI.Clear();
    }

    private static void OverlayUI(GridItem slot) {
        InteractibleAsAbility interaction = GetInteractions(slot);
        for (int i = 0; i < interaction.interactions.Count; i++) {
            if (interaction.interactions[i].GetType() == typeof(Combustible)) {
                Transform t = Instantiate(m.combustibleUIPref, slot.transform.position+new Vector3(0,i), new Quaternion(), m.canvasParent);
                t.gameObject.GetComponent<ButtonInteraction>().interaction = interaction.interactions[i];
                t.gameObject.GetComponent<ButtonInteraction>().source = slot.fillAsStructure;
                m.AddInteraction(t);
            }
        }
    }

    internal static void HideInteractions() {
        ClearUI();
    }
}
