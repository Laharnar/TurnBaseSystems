using UnityEngine;
/// <summary>
/// Base for defining different types of interactions.
/// Like combustion interaction. Or drain. Or pickup. 
/// Connect the SO's to <seealso cref="InteractiveEnvirounment"/> scripts.
/// </summary>
public abstract class Interaction:ScriptableObject {
    public abstract void Interact(IInteractible other);
}
