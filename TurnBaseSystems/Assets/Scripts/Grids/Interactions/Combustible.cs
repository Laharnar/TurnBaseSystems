using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Combustible", menuName = "Grids/Combustible", order = 1)]
public class Combustible : Interaction {
    public override void Interact(IInteractible other) {
        if (other as Structure)
            (other as Structure).Destruct();
        else {
            Debug.Log("Nothing to combust");
        }
    }
}
