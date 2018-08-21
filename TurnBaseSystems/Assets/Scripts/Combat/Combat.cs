﻿using System;
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

    public Queue<AbilityInfo> abilitiesQue = new Queue<AbilityInfo>();
    public Queue<CombatEventMask> activatorsQue = new Queue<CombatEventMask>();
    public const int gameRules = 3; // 0: abilities on energy. 1: max 2. 2: abilities on energy and max 2. 3: energy, max 2, max 1 move

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

        // Set up data instances.
        AbilityInfo.Instance = new AbilityInfo(null, new Vector3(), null);
        PlayerTurnData.Instance = new PlayerTurnData();


        Combat.Instance.StartCombatLoop();

        Debug.Log("Initing gameplay manager");
        flags = new List<FlagManager>();
        flags.Add(new FlagManager(new PlayerFlag(), 0));
        flags.Add(new FlagManager(new EnemyFlag(), 1));

        // obsolete?
        AbilityInfo.CurActivator = new CombatEventMask();

        StartCoroutine(AbilityQueHandler());
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

        gameplayUp = StartCoroutine(CombatUpdate());
    }

    IEnumerator CombatUpdate() {
        // wait until start
        yield return null;
        yield return null;

        while (CombatDisplayManager.Instance.calls.Count > 0) {
            yield return null;
        }

        // flash the combat ui screen
        UIManager.m.slideScreenContent = "FIGHT!";
        CombatDisplayManager.Instance.Register(UIManager.m, "ShowSlideScreen", 4.5f, "Combat/ShowBeginCombatScreen");
        yield return new WaitForSeconds(3.5f);

        Vector3[] positions = new Vector3[flags[0].info.units.Count];
        for (int i = 0; i < flags[0].info.units.Count; i++) {
            flags[0].info.units[i].transform.position = flags[0].info.units[i].snapPos;
            positions[i] = flags[0].info.units[i].snapPos;
        }

        // blinking to player to select the unit.
        UIManager.m.indicatorPositions = positions;
        UIManager.m.indicatorTimeout = 1f;
        CombatDisplayManager.Instance.Register(UIManager.m,
            "ShowIndicators_evt", 1f, "MissionManager/show selection indicators");

        CombatDisplayManager.Instance.Register(this, null, 0.8f, "wait");
        CombatDisplayManager.Instance.Register(UIManager.m,
            "ShowIndicators_evt", 1f, "MissionManager/show selection indicators");
        CombatDisplayManager.Instance.Register(this, null, 0.8f, "wait");

        CombatDisplayManager.Instance.Register(UIManager.m,
            "ShowIndicators_evt", 1f, "MissionManager/show selection indicators");

        // start combat

        bool done = false;
        Debug.Log("Started main loop");
        bool started = false;
        while (true) {
            for (int j = 0; j < flags.Count; j++) {
                if (started) {
                    if (j == 1) {
                        UIManager.ShowSlideMsg("-- Enemy turn --", 3.5f, "Combat/end player turn");
                        yield return new WaitForSeconds(3.5f);
                    }
                    if (j == 0) {
                        UIManager.ShowSlideMsg("-- Player turn --", 3.5f, "Combat/end player turn");
                        yield return new WaitForSeconds(2.5f);
                    }
                }
                started = true;

                activeFlagTurn = j;

                CombatEvents.OnTurnStart(flags[j]);
                flags[0].NullifyUnits();
                flags[1].NullifyUnits();
                yield return StartCoroutine(flags[j].controller.FlagUpdate(flags[j]));
                CombatEvents.OnTurnEnd(flags[j]);
                
                Debug.Log("Flag done - " + (j + 1));
                flags[0].NullifyUnits();
                flags[1].NullifyUnits();
                // all player units die --> lose.
                if (GetUnits(0).Count == 0) {
                    yield return StartCoroutine(LoseGame());
                    done = true;
                    break;

                }
                // all waves were cleared --> win.
                if (GetUnits(1).Count == 0 || MissionManager.levelCompleted) {
                    if (j == 1) {
                        WaveManager.m.OnWaveCleared();
                    }
                    if (WaveManager.m.AllWavesCleared() && GetUnits(1).Count == 0) {
                        yield return StartCoroutine(WinGame());
                        done = true;
                        break;
                    }
                }
                if (GetUnits(0).Count == 0) {
                    yield return StartCoroutine(WinGame());// lose
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


    /// <summary>
    /// Global ability registration.
    /// Use always when unit attacks or does something.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="hoveredSlot"></param>
    /// <param name="activeAbility"></param>
    public static void RegisterAbilityUse(Unit unit, Vector3 hoveredSlot, AttackData2 activeAbility) {
        AbilityInfo info = new AbilityInfo(unit, hoveredSlot, activeAbility, AbilityInfo.CurActivator.Copy());
        // sort ability into correct stacks
        Combat.Instance.abilitiesQue.Enqueue(info);
    }

    /// <summary>
    /// Global ability handler... Executes single ability, animations and all.
    /// </summary>
    /// <returns></returns>
    public IEnumerator AbilityQueHandler() {
        
        while (true) {
            if (abilitiesQue.Count == 0) {
                yield return null;
                continue;
            }
            
            AbilityInfo info = abilitiesQue.Dequeue();
            // activates attack. these attacks can add further attacks to be executed.

            info.executingUnit.AttackAction(info);
            // handles, data changes
            // move reaction is executed when attack is move.
            if (info.activeAbility == info.executingUnit.abilities.move2) {// move
                CombatEvents.ReapplyAuras(info.executingUnit.snapPos, info.attackedSlot, info.executingUnit);
            }

            // wait animations
            info.executingUnit.AttackAnimations(info.activeAbility);
            float len = AttackData2.AnimLength(info.executingUnit, info.activeAbility);
            if (len > 0)
                yield return new WaitForSeconds(len);


            // movement

            // effects

            // sound
        }
    }
}