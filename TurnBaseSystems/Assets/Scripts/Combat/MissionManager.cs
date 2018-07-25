using System;
using UnityEngine;

public class MissionManager:MonoBehaviour {
    public static MissionManager m;
    public static bool levelCompleted { get; private set; }

    public bool fastLoad = false;
    public int[] fastLoadTeam;
    public Transform missionEndScreen_child;

    private void Awake() {
        m = this;

        if (GameRun.current == null)
            Init(CharacterLibrary.CreateInstances(fastLoadTeam));
        else {
            Init(CharacterLoader.LoadActiveTeam());
        }
    }

    /// <summary>
    /// Before they are deparented from loader
    /// </summary>
    public void LoadTeamIntoStartArea(Transform[] team) {
        Transform spawnPoints = GameObject.Find("DONTRENAME_Starting point").transform;
        for (int i = 0; i < team.Length && i < spawnPoints.childCount; i++) {
            team[i].transform.position = spawnPoints.GetChild(i).transform.position;
        }

        /* GameObject team = GameObject.Find("*loader");
         if (GameObject.Find("*loader") && GameObject.Find("Starting point")) {
             Transform spawnPoints = GameObject.Find("Starting point").transform;
             playerTeam = new Transform[team.transform.childCount];
             for (int i = 0; i < playerTeam.Length && i < spawnPoints.childCount; i++) {
                 playerTeam[i] = team.transform.GetChild(i);
                 playerTeam[i].transform.position = spawnPoints.GetChild(i).transform.position;

                 playerTeam[i].gameObject.SetActive(true);
                 playerTeam[i].SetParent(null, true);
             }
         }*/
    }

    internal static void OnReachMissionGoal() {
        levelCompleted = true;
    }

    internal void Init(Transform[] teamInsts) {

        LoadTeamIntoStartArea(teamInsts);
        if (missionEndScreen_child)
            missionEndScreen_child.gameObject.SetActive(false);

        CombatManager.m.Init();
        for (int i = 0; i < teamInsts.Length; i++) {
            teamInsts[i].GetComponent<Unit>().Init();
        }
        CombatManager.m.StartCombatLoop();
    }

    public void OnLoadLevelEndScreen() {
        if (MissionManager.m.missionEndScreen_child) {
            MissionManager.m.missionEndScreen_child.gameObject.SetActive(true);
            MissionManager.m.missionEndScreen_child.GetComponentInChildren<TextAccess>().SetText(LevelRewardManager.m.AsText());
        }
        //Debug.Log("Todo: save the faction points into file.");
        SaveLoad.Save();

        //
        
    }

    public void Btn_LoadMainMenu() {
        LoadingManager.ToMainMenu();
    }
}