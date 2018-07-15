using System;
using UnityEngine;


public class MissionManager:MonoBehaviour {
    public static MissionManager m;
    public static bool levelCompleted { get; private set; }

    Transform[] playerTeam;

    /// <summary>
    /// Before they are deparented from loader
    /// </summary>
    public void LoadTeamIntoStartArea() {
        GameObject team = GameObject.Find("*loader");
        if (GameObject.Find("*loader") && GameObject.Find("Starting point")) {
            Transform spawnPoints = GameObject.Find("Starting point").transform;
            playerTeam = new Transform[team.transform.childCount];
            for (int i = 0; i < playerTeam.Length && i < spawnPoints.childCount; i++) {
                playerTeam[i] = team.transform.GetChild(i);
                playerTeam[i].transform.position = spawnPoints.GetChild(i).transform.position;

                playerTeam[i].gameObject.SetActive(true);
                playerTeam[i].SetParent(null, true);
            }
        }
    }

    internal static void OnReachMissionGoal() {
        levelCompleted = true;
    }

    internal void Init(Character[] team) {
        Transform[] teamInsts = CharacterLibrary.CreateInstances(team);

        //LoadTeamIntoStartArea();

        CombatManager.m.Init();
        for (int i = 0; i < teamInsts.Length; i++) {
            teamInsts[i].GetComponent<Unit>().Init();
        }
        CombatManager.m.StartCombatLoop();
    }

}