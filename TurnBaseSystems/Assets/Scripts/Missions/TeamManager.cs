using System.Collections.Generic;
using UnityEngine;
public class TeamManager:MonoBehaviour {
    static Queue<Character> team = new Queue<Character>();
    const int teamSize = 3;
    private void Update() {
        Transform selected = null;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            selected = SelectionManager.GetMouseAsObject();
        }
        if (selected != null) {
            if (selected.name == "Play") {
                if (team.Count == teamSize) {
                    LoadingManager.m.playerPickedTeam = new Character[teamSize];
                    for (int i = 0; i < teamSize; i++) {
                        LoadingManager.m.playerPickedTeam[i] = team.Dequeue();
                    }
                    LoadingManager.m.LoadNextScreen();
                }
            } else {
                team.Enqueue(selected.root.GetComponent<Unit>().AsCharacterData);
                if (team.Count > teamSize) {
                    team.Dequeue();
                }
            }

        }
    }
}

