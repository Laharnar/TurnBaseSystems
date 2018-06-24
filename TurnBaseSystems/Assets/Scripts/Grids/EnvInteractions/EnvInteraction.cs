using UnityEngine;

[CreateAssetMenu(fileName = "Interaction", menuName = "Grids/Interaction", order = 1)]
public class EnvInteraction : Interaction {

    [System.Obsolete("Shouldn't ever be called. not 100%")]
    public override void Interact(IInteractible other) {
        Debug.Log("shouldn't be called.");
    }

}