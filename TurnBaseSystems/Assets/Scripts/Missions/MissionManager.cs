using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MissionManager : MonoBehaviour {
    public static MissionManager m;
    public List<MissionData> missions = new List<MissionData>();
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
            MissionTarget mt = selected.GetComponent<MissionTarget>();
            activeMission = mt.targetMissionId;
            if (activeMission != lastChoice) {
                missionText.SetText(GetMissionById(activeMission).GetDescription());
            } else {
                PlayMission(activeMission);
                activeMission = -1;
            }
            lastChoice = activeMission;

        }
    }

    public void PlayMission(int id) {
        MissionData mission = GetMissionById(id);
        if (mission!=null) {
            Debug.Log("Starting mission " + mission.missionName + " " + mission.missionId + " " + mission.sceneName);
            LoadingManager.m.activeMission = mission;
            LoadingManager.m.LoadNextScreen();
        } else {
            Debug.Log("No mission with id "+id);
        }
    }

    private MissionData GetMissionById(int id) {
        for (int i = 0; i < missions.Count; i++) {
            if (missions[i].missionId == id) {
                return missions[i];
            }
        }
        return null;
    }
}

