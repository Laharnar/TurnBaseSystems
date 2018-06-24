using System;
using System.Collections.Generic;
using UnityEngine;

// Units can use this when scanning the area.
// Should be on put on walls, grass or other stuff.
// Automatically loaded on child of grid item at runtime, via raycasts.
/// <summary>
/// Contains a list of interactions assigned on structures, weapons, walls, etc.
/// Used in connection with <see cref="UIInteractionController"/> to show
/// BUTTONS that activate those interactions.
/// Assign interactions by hand depending on type of env.
/// </summary>
public class InteractiveEnvirounment : MonoBehaviour {

    // stuff that this slot represents/ abilities that can be use on it.
    public List<Interaction> interactions = new List<Interaction>();
        
    public bool HasInteraction(string type) {
        for (int i = 0; i < interactions.Count; i++) {
            if (interactions[i].interactionType == type)
                return true;
        }
        return false;
    }

    public void RemoveByType(string type) {
        for (int i = 0; i < interactions.Count; i++) {
            if (interactions[i].interactionType == type) {
                interactions.RemoveAt(i);
                i--;
            }
        }
    }

    internal IEnumerable<Interaction> Copies() {
        List<Interaction> ites = new List<Interaction>();
        for (int i = 0; i < interactions.Count; i++) {
            Interaction interaction = ScriptableObject.CreateInstance(interactions[i].GetType()) as Interaction;
            interaction.interactionType = interactions[i].interactionType;
            ites.Add(interaction);
        }
        return ites;
    }

    public static InteractiveEnvirounment AttachScript(GameObject go) {
        return go.AddComponent<InteractiveEnvirounment>();
    }
}
