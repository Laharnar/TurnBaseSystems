using UnityEngine;
public class WaveManager :MonoBehaviour{
    public static WaveManager m;
    public Transform[] enemySpawnAreas;

    public int activeWave = 0;
    public Wave[] waves;

    private void Awake() {
        m = this;
        for (int i = 0; i < enemySpawnAreas.Length; i++) {
            if (enemySpawnAreas[i] != null)
                enemySpawnAreas[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public bool AllWavesCleared() {
        return activeWave >= waves.Length;
    }

    public void OnWaveCleared() {
        if (activeWave >= waves.Length) {
            return; // win
        }

        for (int i = 0; i < waves[activeWave].spawnArea.Length && i < waves[activeWave].enemySet.Length; i++) {
            InitEnemies(waves[activeWave].spawnArea[i], waves[activeWave].enemySet[i].enemies, waves[activeWave].enemySet[i].allianceId);
        }
        activeWave++;
    }
    public void OnCombatBegins() {
        OnWaveCleared();
    }

    public void InitEnemies(int spawnArea, int[] enemyteam, int alliance) {
        if (enemySpawnAreas.Length == 0) return;
        Transform[] insts = CharacterLibrary.CreateInstances(enemyteam);

        MissionManager.m.LoadTeamIntoArea(insts, enemySpawnAreas[spawnArea]);
        for (int i = 0; i < insts.Length; i++) {
            insts[i].GetComponent<Unit>().flag.allianceId = alliance;
            insts[i].GetComponent<Unit>().Init();
            insts[i].GetComponent<Unit>().detection.detectedSomeone = true;
        }
    }
}