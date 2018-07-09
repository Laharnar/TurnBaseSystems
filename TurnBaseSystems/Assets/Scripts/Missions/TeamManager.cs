using System.Collections.Generic;
using UnityEngine;
public class TeamManager:MonoBehaviour {
    int lastChoice;
    public int curTeamSize = 0;
    Queue<Transform> team = new Queue<Transform>();
    const int teamSize = 3;
    private void Update() {
        Transform selected = null;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selected = SelectionManager.GetMouseAsObject();
        }
        if (selected != null) {
            if (selected.name == "Play") {
                if (team.Count == teamSize) {
                    LoadingManager.m.team = new Transform[teamSize];
                    for (int i = 0; i < teamSize; i++) {
                        LoadingManager.m.team[i] = team.Dequeue();
                    }
                    LoadingManager.m.LoadNextScreen();
                }
            } else {
                selected = selected.root;
                team.Enqueue(selected);
                curTeamSize++;
                if (team.Count > teamSize) {
                    team.Dequeue();
                    curTeamSize--;
                }
            }

        }
    }
}

