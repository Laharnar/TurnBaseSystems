using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionManager:MonoBehaviour {
    public static MissionManager m;
    public static bool levelCompleted { get; private set; }

    public bool fastLoad = false;
    public int[] fastLoadTeam;
    public int[] fastLoadEnemyTeam;
    public Transform missionEndScreen_child;
    public Text missionEndScreenText;

    private void Start() {
        m = this;

        if (GameRun.current == null)
            Init(CharacterLibrary.CreateInstances(fastLoadTeam));
        else {
            Init(CharacterLoader.LoadActiveTeam());
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            LoadingManager.RestartMission();
        }
    }

    /// <summary>
    /// Before they are deparented from loader
    /// </summary>
    public void LoadTeamIntoArea(Transform[] team, string spawnPointsName) {
        Transform spawnPoints = GameObject.Find(spawnPointsName).transform;
        for (int i = 0; i < team.Length && i < spawnPoints.childCount; i++) {
            if (team[i]) {
                team[i].transform.position =spawnPoints.GetChild(i).transform.position;

                
            }
            else {
                Debug.LogError("Null spawnable unit at " + i);
            }
        }
    }
    public void WalkTeamIn(Transform[] team, string spawnPointsName) {
        Transform spawnPoints = GameObject.Find(spawnPointsName).transform;
        int lastI = 0;
        for (int i = 0; i < team.Length-1 && i < spawnPoints.childCount-1; i++) {
            // scripted move to sp.
            lastI = i;
            team[i].GetComponent<Unit>().scriptedMovePos = spawnPoints.GetChild(i).transform.position;
            team[i].transform.position = new Vector3(-15, 0, 0) + spawnPoints.GetChild(i).transform.position;
            CombatDisplayManager.Instance.Register(team[i].GetComponent<Unit>(),
                "Move", 0.2f, "MissionManager/Walk to start point");
        }
        lastI++;
        team[lastI].GetComponent<Unit>().scriptedMovePos = spawnPoints.GetChild(lastI).transform.position;
        team[lastI].transform.position = new Vector3(-15, 0, 0) + spawnPoints.GetChild(lastI).transform.position;
        CombatDisplayManager.Instance.Register(team[lastI].GetComponent<Unit>(),
            "Move", 2f, "MissionManager/Walk to start point");
    }

    internal static void OnReachMissionGoal() {
        levelCompleted = true;
    }

    internal void Init(Transform[] teamInsts) {

        LoadTeamIntoArea(teamInsts, "DONTRENAME_Starting point");
        WalkTeamIn(teamInsts, "DONTRENAME_Starting point");

        if (missionEndScreen_child)
            missionEndScreen_child.gameObject.SetActive(false);

        Combat.Instance.Init(teamInsts);
        if (WaveManager.m)
            WaveManager.m.OnCombatBegins();
        else Debug.Log("No wave manger in scene");
    }


    public void OnLoadLevelEndScreen() {
        if (MissionManager.m.missionEndScreen_child) {
            MissionManager.m.missionEndScreen_child.gameObject.SetActive(true);
            //MissionManager.m.missionEndScreen_child.GetComponentInChildren<TextAccess>().SetText(LevelRewardManager.m.AsText());
            m.missionEndScreenText.text = Time.timeSinceLevelLoad.ToString();
        }
        //Debug.Log("Todo: save the faction points into file.");
        SaveLoad.Save();

        //
        
    }

    public void Btn_LoadMainMenu() {
        LoadingManager.ToMainMenu();
    }
}
