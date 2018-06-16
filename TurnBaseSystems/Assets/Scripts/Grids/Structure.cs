using System;
using UnityEngine;
/// <summary>
/// Structures can be laid on top of grid items, to make houses and such.
/// Grid items with structures can't be traversed.
/// </summary>
public class Structure : MonoBehaviour {
    internal void Destruct() {
        Destroy(gameObject);
    }
}