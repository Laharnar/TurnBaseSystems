using UnityEngine;
[CreateAssetMenu(fileName = "Pickable", menuName = "Grids/Pickable", order = 1)]
public class Pickable : Interaction {
    public override void Interact(IInteractible other) {
        if (other as Weapon) {
            Unit.activeUnit.EquipAction(other as Weapon);
            Unit.activeUnit.curSlot.DetachPickupFromSlot();
        } else {
            Debug.Log("Nothing to pick");
        }
    }
}