using UnityEngine;
public class Unit :MonoBehaviour, ISlotItem{
    public int gridX, gridY;
    public bool moving = false;
    public Pathing pathing;
    public Animator anim;

    private void Start() {
        GridItem slot = SelectionManager.GetAsSlot(transform.position);
        Move(slot);
    }

    public void Move(GridItem slot) {
        this.gridX = slot.gridX;
        this.gridY = slot.gridY;
        pathing.GoToCoroutine(this, slot.gridX, slot.gridY, GridManager.m);
    }

    public void Move(Vector2 pos) {
        pathing.GoToCoroutine(this, pos, GridManager.m);
    }
}
