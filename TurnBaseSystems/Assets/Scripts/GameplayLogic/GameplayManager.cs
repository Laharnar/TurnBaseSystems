using System;
using System.Collections;
using UnityEngine;
public class GameplayManager : MonoBehaviour {

    public static GameplayManager m;
    int activeFlagTurn = 0;

    Transform[] playerTeam;
    Coroutine gameplayUp;

    private void Awake() {
        m = this;
        Init();
    }

    public void Init() {
        Debug.Log("Initing gameplay manager");
        FlagManager.flags = new System.Collections.Generic.List<FlagController>();
        FlagManager.flags.Add(new PlayerFlag());
        FlagManager.flags.Add(new EnemyFlag());
        if (gameplayUp != null)
            StopCoroutine(gameplayUp);
        gameplayUp=StartCoroutine(GameplayUpdate());
    }
    /// <summary>
    /// Before they are deparented from loader
    /// </summary>
    public void LoadTeam() {
        GameObject team = GameObject.Find("*loader");
        Transform spawnPoints = GameObject.Find("Starting point").transform;
        playerTeam = new Transform[team.transform.childCount];
        for (int i = 0; i < playerTeam.Length && i < spawnPoints.childCount; i++) {
            playerTeam[i] = team.transform.GetChild(i);
            playerTeam[i].transform.position = spawnPoints.GetChild(i).transform.position;
        }
    }
    IEnumerator GameplayUpdate() {
        yield return null;
        bool done = false;
        Debug.Log("Started main loop");
        while (true) {
            for (int j = 0; j < FlagManager.flags.Count; j++) {
                activeFlagTurn = j;

                for (int i = 0; i < FlagManager.flags[j].units.Count; i++) {
                    FlagManager.flags[j].units[i].OnTurnStart();
                }
                yield return StartCoroutine(FlagManager.flags[j].FlagUpdate());

                for (int i = 0; i < FlagManager.flags[j].units.Count; i++) {
                    FlagManager.flags[j].units[i].OnTurnEnd();
                }
                Debug.Log("Flag done - " + (j + 1));
                FlagManager.flags[j].NullifyUnits();
                if (FlagManager.flags[0].units.Count == 0) {
                    yield return StartCoroutine(LoseGame());
                    done = true;
                    break;

                }
                // temp - win condition that enemy dies.
                if (FlagManager.flags[1].units.Count == 0) {
                    yield return StartCoroutine(WinGame());
                    done = true;
                    break;
                }
                yield return new WaitForSeconds(0.5f);

            }
            if (done) {
                break;
            }
            
            yield return null;
        }
        Debug.Log("Exited main loop");
    }

    private IEnumerator WinGame() {
        Debug.Log("WIN!");
        yield return null;
    }

    private IEnumerator LoseGame() {
        Debug.Log("Lose!");
        yield return null;
    }
}
