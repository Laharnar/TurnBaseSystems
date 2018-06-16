using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Combustible", menuName = "Grids/Combustible", order = 1)]
public class Combustible : Interaction {
    public override void Interact(Structure other) {
        if (other)
            other.Destruct();
        else {
            Debug.Log("Nothing to combust");
        }
    }
}