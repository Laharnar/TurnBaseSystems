using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

    public static Combat Instance { get { return instance; } }
    static Combat instance;

    int activeFlagTurn = 0;

    Coroutine gameplayUp;
    public bool initAwake = true;
    bool init;

    /// <summary>
    /// Don't edit outside this script.
    /// </summary>
    [System.Obsolete("don't use, complex")]
    public int mouseDirection = 0;

    public List<Unit> units = new List<Unit>();
    internal int lastMouseDirection;

    public List<FlagManager> flags;

    private void Awake() {
        instance = this;
        if (initAwake) {
            Init(new Transform[0]);
            // StartCombatLoop ();
        }
    }

    public void Init(Transform[] teamInsts) {

        if (init) return;
        init = true;

        for (int i = 0; i < teamInsts.Length; i++) {
            teamInsts[i].GetComponent<Unit>().Init();
        }

        Combat.Instance.StartCombatLoop();

        Debug.Log("Initing gameplay manager");
        flags = new List<FlagManager>();
        flags.Add(new FlagManager(new PlayerFlag(), 0));
        flags.Add(new FlagManager(new EnemyFlag(), 1));

        CI.curActivator = new CombatEventMask();
    }

    public void RegisterUnit(Unit u) {
        /*if (flags.Count < u.flag.allianceId) {
            Init();
        }*/
        GetUnits(u.flag.allianceId).Add(u);
        units.Add(u);
    }

    internal void DeRegisterUnit(Unit u) {
        GetUnits(u.flag.allianceId).Remove(u);
        units.Remove(u);
    }

    public List<Unit> GetUnits(int allianceId) {
        return flags[allianceId].info.units;
    }

    public void StartCombatLoop() {
        if (gameplayUp != null)
            StopCoroutine(gameplayUp);

        gameplayUp = StartCoroutine(GameplayUpdate());
    }

    IEnumerator GameplayUpdate() {
        // wait until start
        yield return null;

        // flash the combat ui screen
        UIManager.m.slideScreenContent = "FIGHT!";
        CombatDisplayManager.Instance.Register(UIManager.m, "ShowSlideScreen", 1f, "Combat/ShowBeginCombatScreen");
        yield return new WaitForSeconds(1f);

        bool done = false;
        Debug.Log("Started main loop");
        while (true) {
            for (int j = 0; j < flags.Count; j++) {
                activeFlagTurn = j;

                CombatEvents.OnTurnStart(flags[j]);

                flags[j].NullifyUnits();
                yield return StartCoroutine(flags[j].controller.FlagUpdate(flags[j]));


                CombatEvents.OnTurnEnd(flags[j]);

                Debug.Log("Flag done - " + (j + 1));
                flags[j].NullifyUnits();
                if (GetUnits(0).Count == 0) {
                    yield return StartCoroutine(LoseGame());
                    done = true;
                    break;

                }
                // temp - win condition that enemy dies.
                if (GetUnits(1).Count == 0 || MissionManager.levelCompleted) {
                    if (j == 1) {
                        WaveManager.m.OnWaveCleared();
                    }
                    if (WaveManager.m.AllWavesCleared()&& GetUnits(1).Count == 0) {
                        yield return StartCoroutine(WinGame());
                        done = true;
                        break;
                    }
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

    internal void SkipWave() {
        for (int i = 0; i < GetUnits(1).Count; i++) {
            Destroy(GetUnits(1)[i].gameObject);
        }
        GetUnits(1).Clear();
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

    internal void UnitNullCheck() {
        for (int i = 0; i < units.Count; i++) {
            if (units[i] == null) {
                units.RemoveAt(i);
                i--;
            }
        }
    }
}
