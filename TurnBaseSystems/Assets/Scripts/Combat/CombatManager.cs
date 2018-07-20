using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CombatManager : MonoBehaviour {

    public static CombatManager m;
    int activeFlagTurn = 0;

    Coroutine gameplayUp;

    bool init;


    private void Awake() {
        m = this;
        Init();
        StartCombatLoop();
    }

    public void Init() {

        if (init) return;
        init = true;

        Debug.Log("Initing gameplay manager");
        FlagManager.flags = new System.Collections.Generic.List<FlagController>();
        FlagManager.flags.Add(new PlayerFlag());
        FlagManager.flags.Add(new EnemyFlag());

    }

    public void StartCombatLoop() {
        if (gameplayUp != null)
            StopCoroutine(gameplayUp);

        gameplayUp = StartCoroutine(GameplayUpdate());
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
                OnTurnEnd(j);

                Debug.Log("Flag done - " + (j + 1));
                FlagManager.flags[j].NullifyUnits();
                if (FlagManager.flags[0].units.Count == 0) {
                    yield return StartCoroutine(LoseGame());
                    done = true;
                    break;

                }
                // temp - win condition that enemy dies.
                if (FlagManager.flags[1].units.Count == 0 || MissionManager.levelCompleted) {
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

    private void OnTurnEnd(int j) {
         BuffManager.ConsumeBuffs(j);
    }

    public static void OnUnitExecutesAction(Unit unit) {
        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
    }

    internal static void OnEnterCheckpoint(FactionCheckpoint checkpoint, Unit unit) {
        LevelRewardManager.AddReward(checkpoint.reward, unit);
        if (checkpoint.isMissionGoal) {
            MissionManager.OnReachMissionGoal();
        }
    }

    private IEnumerator WinGame() {
        Debug.Log("WIN!");
        MissionManager.m.OnLoadLevelEndScreen();
        yield return null;
    }

    private IEnumerator LoseGame() {
        Debug.Log("Lose!");
        yield return null;

    }

 
}
