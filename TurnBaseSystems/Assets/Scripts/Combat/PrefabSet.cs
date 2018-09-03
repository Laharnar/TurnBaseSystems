using UnityEngine;
[CreateAssetMenu(fileName = "EnemySet", menuName = "Waves/EnemySet", order = 1)]
public class PrefabSet :ScriptableObject{
    public string description;
    public int allianceId=1;// lets you spawn enemies and allies in same wave.
    public int[] enemies;
}
