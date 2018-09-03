using UnityEngine;
[CreateAssetMenu(fileName = "EnemySet", menuName = "Waves/EnemySet", order = 1)]
public class PrefabSet :ScriptableObject{
    public string description;
    public int[] enemies;
}
