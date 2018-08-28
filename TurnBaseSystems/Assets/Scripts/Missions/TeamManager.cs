using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager:MonoBehaviour {
    public List<Character> team = new List<Character>();
    const int teamSize = 3;

    public Transform[] avaliableTeam;

    // ui
    public Transform[] onSelectedMarkers;

    private void Start() {
        LoadAvaliableTeam();
        HideMarkers();
    }

    private void HideMarkers() {
        for (int i = 0; i < onSelectedMarkers.Length; i++) {
            onSelectedMarkers[i].gameObject.SetActive(false);
        }
    }

    private void LoadAvaliableTeam() {
        Transform[] objs = CharacterLoader.LoadUnlockedCharacters();

        for (int i = 0; i < objs.Length; i++) {
            objs[i].transform.position = new Vector3(i*5-10, 0, 0);
        }
        //avaliableTeam = objs;
    }

    private void Update() {
        // selection process
        Transform selected = null;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selected = SelectionManager.GetMouseAsObject();
        }
        if (selected != null) {
            if (selected.name == "Play") {
                if (team.Count == teamSize) {
                    OnPressPlay();
                }
            } else if (NotDouble(selected.root)) {
                Debug.Log("Selected unit");
                
                team.Add(selected.root.GetComponent<Unit>().AsCharacterData);
                OnPressUnit(GetIdByPos(selected.root.transform));
                if (team.Count > teamSize) {
                    // find unit by code and remove it
                    for (int i = 0; i < avaliableTeam.Length; i++) {
                        if (avaliableTeam[i].GetComponent<Unit>().codename == team[0].name) {
                            OnDeselectUnit(GetIdByPos(avaliableTeam[i].transform));
                            break;
                        }
                    }

                    team.RemoveAt(0);
                }
            }
        }
    }

    private bool NotDouble(Transform root) {
        for (int i = 0; i < team.Count; i++) {
            if (team[i].name == root.GetComponent<Unit>().codename) {
                return false;
            }
        }
        return true;
    }

    private void OnPressPlay() {
        Character[] playerPickedTeam = new Character[teamSize];
        for (int i = 0; i < teamSize; i++) {
            playerPickedTeam[i] = team[i]; //team.Dequeue();
        }
        team.Clear();
        GameRun.current.currentMap.activeTeam = playerPickedTeam;
        LoadingManager.m.OnLoadMission();
    }

    private void OnDeselectUnit(int id) {
        onSelectedMarkers[id].gameObject.SetActive(false);
    }

    private int GetIdByPos(Transform transform) {
        for (int i = 0; i < avaliableTeam.Length; i++) {
            if (avaliableTeam[i] == transform) {
                return i;
            }
        }
        return -1;
    }

    public void OnPressUnit(int i) {
        onSelectedMarkers[i].gameObject.SetActive(true);
    }
}

