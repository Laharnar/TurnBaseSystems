﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour {

    public static CombatManager m;
    int activeFlagTurn = 0;

    Coroutine gameplayUp;

    bool init;

    public List<Unit> units = new List<Unit>();

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

                OnTurnStart(j);
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

    private void OnTurnStart(int allianceId) {
        for (int i = 0; i < units.Count; i++) {
            for (int j = 0; j < units[i].abilities.additionalAbilities2.Count; j++) {
                EmpowerAlliesData aura = units[i].abilities.additionalAbilities2[j].aura;
                if (units[i].abilities.additionalAbilities2[j].aura.used) {
                    aura.DeEffectArea(units[i].snapPos);
                    aura.EffectArea(units[i].snapPos);
                }
            }
        }  
    }

    private void OnTurnEnd(int j) {
         BuffManager.ConsumeBuffs(j);
    }

    public static void OnUnitExecutesAction(Unit unit) {
        foreach (var items in FactionCheckpoint.checkpointsInLevel) {
            items.CheckpointCheck(unit);
        }
        CombatManager.m.UnitNullCheck();
    }

    public static void CombatAction(Unit selectedPlayerUnit, Vector3 hoveredSlot, AttackData2 activeAbility) {
        Vector3 curPos = GridManager.SnapPoint(selectedPlayerUnit.transform.position);
        int action = selectedPlayerUnit.AttackAction2(hoveredSlot, activeAbility);
        if (action == 1) {// move
            OnUnitExecutesMoveAction(curPos, hoveredSlot, selectedPlayerUnit);
        }
    }

    public static void OnUnitExecutesMoveAction(Vector3 oldPos, Vector3 newPos, Unit unit) {
        // on enter - on exit.
        
        // has to get all auras in old pos, and all auras in new pos, then apply new effect and lose old
        // for unit pos get all auras - all units - all abilities - in mask check - save m*n
        foreach (var combatUnit in m.units) {
            Vector3 snap = GridManager.SnapPoint(combatUnit.transform.position);
            foreach (var ability in combatUnit.abilities.additionalAbilities2) {
                if (ability.aura.used) {
                    bool inOld = ability.aura.auraRange.IsPosInMask(snap, oldPos);
                    bool inNew = ability.aura.auraRange.IsPosInMask(snap, newPos);
                    bool changedAura=oldPos!=newPos;
                    if (inOld && !inNew) {
                        ability.aura.LoseEffect(unit);
                    }
                    if (!inOld && inNew) {
                        ability.aura.Effect(unit);
                    }
                }
            }
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

    internal void UnitNullCheck() {
        for (int i = 0; i < units.Count; i++) {
            if (units[i] == null) {
                units.RemoveAt(i);
                i--;
            }
        }
    }
}
