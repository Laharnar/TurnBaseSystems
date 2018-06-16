﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractionController:MonoBehaviour {

    public static UIInteractionController m;

    List<Transform> generatedUI = new List<Transform>();

    public Transform canvasParent;

    public Transform combustibleUIPref;

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
        for (int i = 0; i < m.generatedUI.Count; i++) {
            if (m.generatedUI[i])
            GameObject.Destroy(m.generatedUI[i].gameObject);
        }
        m.generatedUI.Clear();
    }

    private static void OverlayUI(GridItem slot) {
        InteractibleAsAbility interaction = slot.avaliableAbilities;
        for (int i = 0; i < interaction.interactions.Count; i++) {
            if (interaction.interactions[i].GetType() == typeof(Combustible)) {
                Transform t = Instantiate(m.combustibleUIPref, slot.transform.position+new Vector3(0,i), new Quaternion(), m.canvasParent);
                ButtonInteraction bi = t.gameObject.GetComponent<ButtonInteraction>();
                bi.interaction = interaction.interactions[i];
                bi.source = slot.fillAsStructure;
                m.AddUIPiece(t);
            }
        }
    }

    internal static void HideInteractions() {
        ClearUI();
    }
}