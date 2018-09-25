using System;
using System.Collections.Generic;
using UnityEngine;
public class WaveManager :MonoBehaviour{
    public static WaveManager m;
    public Transform[] enemySpawnAreas;

    internal int activeWave = -1;
    public List<Wave> waves = new List<Wave>();

    float lastWaveClearedTime = 0f;

    public string curWaveDescription {
        get {
            return waves[activeWave].name;
        }
    }

    private void Awake() {
        m = this;
        for (int i = 0; i < enemySpawnAreas.Length; i++) {
            if (enemySpawnAreas[i] != null)
                enemySpawnAreas[i].GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Comma)) {
            Combat.Instance.SkipWave();
            if (activeWave >= waves.Count) {
                return; // win
            }
            activeWave++;
        }
    }
    public bool AllWavesCleared() {
        return activeWave >= waves.Count;
    }
    public void PrintWave() {
        Debug.Log("----- WAVE " + (activeWave+1) + " (last wave time: " + lastWaveClearedTime + " level time: " + Time.timeSinceLevelLoad + ") " + waves[activeWave].description + " STARTED -----");
    }

    public void OnWaveCleared() {
        lastWaveClearedTime = Time.timeSinceLevelLoad-lastWaveClearedTime;
        if (activeWave + 1 >= waves.Count) {
            activeWave++;
            return; // win
        }
        activeWave++;
        PrintWave();

        for (int i = 0; i < waves[activeWave].spawnArea.Length && i < waves[activeWave].enemySet.Length; i++) {
            InitEnemies(waves[activeWave].spawnArea[i], waves[activeWave].enemySet[i].enemies, waves[activeWave].enemySet[i].allianceId);
        }
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
[System.Serializable]
public class SkillLockdown {
    public bool notUsed = false;
    public int[] unlockAtLevel;

    public bool IsSkillUnlocked(int skillId, int curWave) {
        return notUsed ||
            (skillId < unlockAtLevel.Length && curWave >= unlockAtLevel[skillId]);
    }
}