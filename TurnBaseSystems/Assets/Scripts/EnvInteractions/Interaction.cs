using System;
using UnityEngine;
/// <summary>
/// Base for defining different types of interactions.
/// Like combustion interaction. Or drain. Or pickup. 
/// Connect the SO's to <seealso cref="InteractiveEnvirounment"/> scripts on envriounment 
/// or pickups.
/// Note: only type is important so this could be removed for standard string check
/// </summary>
public abstract class Interaction:ScriptableObject {
    [System.Obsolete("Shouldn't ever be called. not 100%")]
    public abstract void Interact(IInteractible other);

    public string interactionType = "Normal";
    
}
