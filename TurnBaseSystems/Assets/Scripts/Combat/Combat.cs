using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public partial class Combat : MonoBehaviour {

    public static Combat Instance { get { return GameManager.Instance.GetManager<Combat>(); } }
    //static Combat instance;

    Coroutine gameplayUp;
    public bool initAwake = true;
    bool init;

    public FlagManager flagsManager { get { return GameManager.Instance.GetManager<FlagManager>(); } }
    public List<Unit> units { get { return flagsManager.Units; } }
    public List<Flag> flags { get { return flagsManager.flags; } set { flagsManager.flags = value; } }

    public CombatAbilityHandler1 abilityHandler;
    public const int gameRules = 3; // 0: abilities on energy. 1: max 2. 2: abilities on energy and max 2. 3: energy, max 2, max 1 move. 4: max 1 attack, max 1 move

    public SkillLockdown lockdown;

    public int activeFlag = 0;
    private string enemyTurnAlternateText = "";

    private void Awake() {
        //instance = this;
        FlagManager.InitInstance();

        GameManager.Instance.RegisterManager(this, 2);
        if (initAwake) {
            Init(new Transform[0]);
            // StartCombatLoop ();
        }
    }
    private void Start() {
        //TempGroundGrid(transform.position);
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

        // start combat loop
        if (gameplayUp != null)
            StopCoroutine(gameplayUp);
        gameplayUp = StartCoroutine(CombatUpdate());

        Debug.Log("Initing gameplay manager");
        flags = new List<Flag>();
        flags.Add(new Flag(new PlayerFlag(), 0));
        flags.Add(new Flag(new EnemyFlag(), 1));

        // obsolete?
        AbilityInfo.CurActivator = new CombatEventMask();

        abilityHandler = new CombatAbilityHandler1();
        StartCoroutine(abilityHandler.AbilityQueHandler());
    }

    public void RegisterUnit(Unit u) {
        /*if (flags.Count < u.flag.allianceId) {
            Init();
        }*/
        flagsManager.AddUnit(u, u.flag.allianceId);
        //units.Add(u);
    }

    internal void Reset() {
        AbilityInfo.Instance.Reset();
    }

    internal void DeRegisterUnit(Unit u) {
        GetUnits(u.flag.allianceId).Remove(u);
        units.Remove(u);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="allianceId"></param>
    /// <param name="onlyVisible">f: you can use it as reference. T: it's a copy</param>
    /// <returns></returns>
    public List<Unit> GetUnits(int allianceId) {
        return flags[allianceId].info.units;
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

        // snap all player units that walked in.
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
                activeFlag = j;
                if (started) {
                    if (j == 1) {
                        if (GetUnits(1).Count == 0)
                            enemyTurnAlternateText = "-- Wave "+(WaveManager.m.NextWaveDescription)+" ("+ (WaveManager.m.activeWave+1) + "/"+(WaveManager.m.waves.Count)+") --";
                        if (enemyTurnAlternateText != "") {
                            UIManager.ShowSlideMsg(enemyTurnAlternateText, 4.5f, "Combat/end player turn");
                            enemyTurnAlternateText = "";
                        } else {
                            UIManager.ShowSlideMsg("-- Enemy turn --", 2.5f, "Combat/end player turn");
                        }
                        yield return new WaitForSeconds(2.5f);
                    }
                    if (j == 0) {
                        UIManager.ShowSlideMsg("-- Player turn --", 2.5f, "Combat/end player turn");
                        yield return new WaitForSeconds(1.75f);
                    }
                }
                started = true;


                CombatEvents.OnTurnStart(flags[j]);
                flags[0].NullifyUnits();
                flags[1].NullifyUnits();
                yield return new WaitForSeconds(1);
                yield return StartCoroutine(flags[j].controller.FlagUpdate(flags[j]));
                yield return new WaitForSeconds(1);
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
                        //enemyTurnAlternateText = "-- Wave "+(WaveManager.m.curWaveDescription)+"/"+(WaveManager.m.waves.Count)+" --";
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

    internal void RunVfx(Vector3 attackStartedAt, object vfx) {
        throw new NotImplementedException();
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
        flagsManager.UnitNullCheck();
    }


    /// <summary>
    /// Global ability registration.
    /// Use always when unit attacks or does something.
    /// 
    /// Describes how combat manager, handles ability activations.
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="hoveredSlot"></param>
    /// <param name="activeAbility"></param>
    public static void RegisterAbilityUse(Unit unit, Vector3 hoveredSlot, AttackData2 activeAbility, CombatEventMask activator) {
        AbilityInfo info = new AbilityInfo(unit, hoveredSlot, activeAbility, activator);
        // sort ability into correct stacks
        Combat.Instance.abilityHandler.AddAbility(info);
    }


    public static bool ShouldAbilityBeLocked(int abilityId) {
        if (Instance.lockdown.IsSkillUnlocked(abilityId, WaveManager.m.activeWave)) {
            return false;
        }
        return true;
    }
}

/// <summary>
/// VFX Combat control.
/// </summary>
public partial class Combat : MonoBehaviour {
    public int vfxCount = 0;

    public void RunVfx(Vector3 pos, Transform pref) {
        if (pref == null) {
            return;
        }
        vfxCount++;
        VfxController vfxc = Instantiate(pref, pos, new Quaternion()).GetComponent<VfxController>();
        vfxc.Init(EndVfx);
    }

    public bool EndedAllVfx() {
        return vfxCount == 0;
    }

    public void EndVfx() {
        vfxCount--;
    }
}