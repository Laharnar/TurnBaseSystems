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
    public void LoadTeamIntoArea(Transform[] team, Transform spawnPointParent) {
        
        Transform spawnPoints = spawnPointParent;
        for (int i = 0; i < team.Length && i < spawnPoints.childCount; i++) {
            if (team[i]) {
                team[i].transform.position =spawnPoints.GetChild(i).transform.position;
            }
            else {
                Debug.LogError("Null spawnable unit at " + i);
            }
        }
    }
    public void WalkTeamIn(Transform[] team, Transform spawnPointParent) {
        // init team pos
        Transform spawnPoints = spawnPointParent;
        for (int i = 0; i < team.Length && i < spawnPoints.childCount; i++) {
            // scripted move to sp.
            team[i].GetComponent<Unit>().scriptedMovePos = spawnPoints.GetChild(i).transform.position;
            team[i].transform.position = new Vector3(-20, 0, 0) + spawnPoints.GetChild(i).transform.position;
        }

        // focus on player walking in... wait, move, wait
        CombatDisplayManager.Instance.Register(this,
                null, 1.75f, "MissionManager/Wait a bit");
        for (int i = 0; i < team.Length; i++) {
            CombatDisplayManager.Instance.Register(team[i].GetComponent<Unit>(),
                "Move", 0.2f, "MissionManager/Walk to start point");
        }
        CombatDisplayManager.Instance.Register(this,
            null, 1.8f, "MissionManager/Wait after walk");

    }

    internal static void OnReachMissionGoal() {
        levelCompleted = true;
    }

    internal void Init(Transform[] teamInsts) {
        GameObject go = GameObject.Find("DONTRENAME_Starting point");
        if (go == null) {
            Debug.Log("ERROR:MISSING object " + "DONTRENAME_Starting point" + " aborting ally load.");
            return;
        } else {
            LoadTeamIntoArea(teamInsts, go.transform);
            WalkInCamera(teamInsts);
            WalkTeamIn(teamInsts, go.transform);
        }

        if (missionEndScreen_child)
            missionEndScreen_child.gameObject.SetActive(false);

        Combat.Instance.Init(teamInsts);

        if (WaveManager.m)
            WaveManager.m.OnCombatBegins();
        else Debug.Log("No wave manger in scene");


        // focus on enemies, shortly
        Vector3 center = ((Transform[])Combat.Instance.flags[1].info).Length > 0 ?
            GetCenter((Transform[])Combat.Instance.flags[1].info) : new Vector3();
        GameManager.Instance.combatCam.AddPos(center);
        CombatDisplayManager.Instance.Register(GameManager.Instance.combatCam,
            "MoveToPos", GameManager.Instance.combatCam.RequiredTimeToMoveToPos(center) + 0.5f, "MissionManager/focus on enemy");

        // focus back on player
        center = GetCenter(teamInsts) + new Vector3(20, 0, 0);
        GameManager.Instance.combatCam.AddPos(center);
        CombatDisplayManager.Instance.Register(GameManager.Instance.combatCam,
            "MoveToPos", GameManager.Instance.combatCam.RequiredTimeToMoveToPos(center) + 0.5f, "MissionManager/focus on player");

    }
    public static Vector3 GetCenter(Transform[] positions) {
        Vector3 center = new Vector3();
        for (int i = 0; i < positions.Length; i++) {
            center += positions[i].transform.position;
        }
        center = (center)/ positions.Length;
        return center;
    }
    private void WalkInCamera(Transform[] team) {
        GameManager.Instance.combatCam.transform.position = GetCenter(team);
        GameManager.Instance.combatCam.FollowCenterPos(team, 5);
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

    // referenced on buttons
    public void Btn_LoadMainMenu() {
        LoadingManager.ToMainMenu();
    }
}
