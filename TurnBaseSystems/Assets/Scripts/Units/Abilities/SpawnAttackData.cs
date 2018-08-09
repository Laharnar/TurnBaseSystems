using UnityEngine;
[System.Serializable]
public class SpawnAttackData : AbilityEffect {
    public Transform spawnItemPref;
    public SlotContent requirments = SlotContent.Empty;
    public void SpawnItem(Vector3 pos) {
        Transform t = GameObject.Instantiate(spawnItemPref);
        t.transform.position = GridManager.SnapPoint(pos);
    }

    internal override void AtkBehaviourExecute() {
        Execute();
    }

    public void Execute() {
        if (GridManager.ValidSlot(CI.attackedSlot, requirments))
            SpawnItem(CI.attackedSlot);
    }
}
