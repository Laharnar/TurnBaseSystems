using UnityEngine;
/// <summary>
/// Units can use this when
/// </summary>
public abstract class Interaction:ScriptableObject {
    public abstract void Interact(Structure other);
}

public interface IInteractible {

}