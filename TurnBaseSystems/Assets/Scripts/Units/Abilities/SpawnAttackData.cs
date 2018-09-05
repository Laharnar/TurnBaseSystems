using UnityEngine;
[System.Serializable]
public class SpawnAttackData : AbilityEffect {
    public Transform spawnItemPref;
    public SlotContent requirments = SlotContent.Empty;
    public void SpawnItem(Vector3 pos) {
        Transform t = GameObject.Instantiate(spawnItemPref);
        t.transform.position = GridManager.SnapPoint(pos);
    }

    internal override void AtkBehaviourExecute(AbilityInfo info) {
        if (info.activator.onAttack) {
            Execute(info);
            info.executingUnit.AbilitySuccess();
        }
    }

    public void Execute(AbilityInfo info) {
        if (GridManager.ValidSlot(info.attackedSlot, requirments))
            SpawnItem(info.attackedSlot);
    }
}
