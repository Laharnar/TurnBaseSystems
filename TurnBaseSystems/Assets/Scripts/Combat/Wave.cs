using UnityEngine;
[CreateAssetMenu(fileName = "Wave", menuName = "Waves/Wave", order = 1)]
public class Wave:ScriptableObject {
    public string description;
    public string additionalTrigger;

    public int[] spawnArea;
    public PrefabSet[] enemySet;

    //public int[] enemies;

    public void RunTrigger() {
        MissionManager.m.Invoke(additionalTrigger, 0f);
    }
}
