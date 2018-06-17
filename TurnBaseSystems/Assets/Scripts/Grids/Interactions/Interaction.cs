using UnityEngine;
/// <summary>
/// Base for defining what kind of interactions some object has.
/// Like combustion interaction. Or drain.
/// </summary>
public abstract class Interaction:ScriptableObject {
    public abstract void Interact(IInteractible other);
}
