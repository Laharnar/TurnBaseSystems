using System;
using System.Collections.Generic;
using UnityEngine;

// Units can use this when scanning the area.
// Should be on put on walls, grass or other stuff.
// Automatically loaded on child of grid item at runtime, via raycasts.
/// <summary>
/// Contains a list of interactions assigned either on structures, weapons, walls, etc.
/// Used in connection with <see cref="UIInteractionController"/> to show
/// BUTTONS that activate interactions.
/// </summary>
public class InteractiveEnvirounment : MonoBehaviour {

    // stuff that this slot represents/ abilities that can be use on it.
    public List<Interaction> interactions = new List<Interaction>();
        
    public bool HasInteraction<T>() where T: Interaction {
        for (int i = 0; i < interactions.Count; i++) {
            if (interactions[i].GetType() == typeof(T))
                return true;
        }
        return false;
    }

    internal IEnumerable<Interaction> Copies() {
        List<Interaction> ites = new List<Interaction>();
        for (int i = 0; i < interactions.Count; i++) {
            ites.Add(ScriptableObject.CreateInstance(interactions[i].GetType()) as Interaction);
        }
        return ites;
    }

    public static InteractiveEnvirounment AttachScript(GameObject go) {
        return go.AddComponent<InteractiveEnvirounment>();
    }
}
