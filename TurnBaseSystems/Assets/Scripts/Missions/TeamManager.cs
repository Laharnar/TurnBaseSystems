using System;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager:MonoBehaviour {
    public List<Character> team = new List<Character>();
    const int teamSize = 3;

    private void Start() {
        LoadAvaliableTeam();
    }
    
    private void LoadAvaliableTeam() {
        Transform[] objs = CharacterLoader.LoadUnlockedCharacters();

        for (int i = 0; i < objs.Length; i++) {
            objs[i].transform.position = new Vector3(i*5-10, 0, 0);
        }
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
            } else {
                Debug.Log("Selected unit");
                team.Add(selected.root.GetComponent<Unit>().AsCharacterData);
                if (team.Count > teamSize) {
                    team.RemoveAt(0);
                }
            }

        }
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
}

