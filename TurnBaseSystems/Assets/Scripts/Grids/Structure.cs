﻿using System;
using UnityEngine;
/// <summary>
/// Structures can be laid on top of grid items, to make houses and such.
/// Grid items with structures can't be traversed.
/// </summary>
public class Structure : MonoBehaviour,IInteractible {

    public int materialValue = 5;
    /*public void OnUnitInteracts() {
        // combustible
        Destruct();
    }*/

    internal void Destruct() {
        Destroy(gameObject);
    }

    internal void Consume() {
        Unit.activeUnit.materials += materialValue;
        Destroy(gameObject);
    }
}