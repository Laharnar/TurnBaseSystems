using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapMissionManager : MonoBehaviour {
    public static MapMissionManager m;
    public QuestData[] missions;
    public TextAccess missionText;

    int activeMission = -1;
    int lastChoice;

    private void Awake() {
        m = this;
    }

    // Update is called once per frame
    void Update () {

        Transform selected = null;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selected = SelectionManager.GetMouseAsObject();
        }
        if (selected != null) {
            // choose active mission
            MissionTarget mt = selected.GetComponent<MissionTarget>();
            activeMission = mt.targetMissionId;
            if (activeMission != lastChoice) {
                missionText.SetText(GetQuestDataById(activeMission).GetDescription());
            } else {
                PlayMission(activeMission);
                activeMission = -1;
            }
            lastChoice = activeMission;

        }
    }

    public void PlayMission(int id) {
        QuestData mission = GetQuestDataById(id);
        if (mission!=null) {
            Debug.Log("Starting mission " + mission.missionName + " " + mission.missionId + " " + mission.sceneName);
            GameRun.current.activeQuest = mission;
            LoadingManager.OnLoadCharacterScreen();
        } else {
            Debug.Log("No mission with id "+id);
        }
    }

    private QuestData GetQuestDataById(int id) {
        for (int i = 0; i < missions.Length; i++) {
            if (missions[i].missionId == id) {
                return missions[i];
            }
        }
        return null;
    }
}

