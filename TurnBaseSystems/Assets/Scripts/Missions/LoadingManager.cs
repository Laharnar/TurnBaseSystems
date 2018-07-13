using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingManager : MonoBehaviour {
    public static LoadingManager m;
    internal MissionData activeMission;
    internal Transform[] team;

    int activeLoadingScreen = -1;

    public Transform missionEndScreen_child;

    private void Awake() {
        if (m != null)
            Destroy(gameObject);
        else {
            m = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnLoadCharacterScreen() {
        activeLoadingScreen = 1;
        SceneManager.LoadScene("teamSelection");
    }



    private void OnLoadMission() {
        activeLoadingScreen = 2;
        for (int i = 0; i < team.Length; i++) {
            team[i].parent = transform;
            team[i].gameObject.SetActive(false);
        }
        SceneManager.LoadScene(activeMission.sceneName);
        StartCoroutine(WaitSceneLoad());

    }

    private IEnumerator WaitSceneLoad() {
        yield return new WaitForSeconds(2);
        CombatManager.m.Init();
        CombatManager.m.LoadTeam();
        for (int i = 0; i < team.Length; i++) {
            team[i].gameObject.SetActive(true);
            team[i].SetParent(null, true);
            team[i].GetComponentInChildren<HpUIController>().OnLoadedMission();
            team[i].GetComponent<Unit>().Init();
        }

    }

    public void LoadNextScreen() {
        missionEndScreen_child.gameObject.SetActive(false);
        if (activeLoadingScreen == -1) {
            OnLoadMap();
        } else if (activeLoadingScreen == 0) {
            OnLoadCharacterScreen();
        } else if (activeLoadingScreen == 1) {
            OnLoadMission();
        } else if (activeLoadingScreen == 2) {
            OnLoadLevelEndScreen();
        }
    }

    private void OnLoadMap() {
        activeLoadingScreen = 0;
        // TODO: Move this to main menu later, don't forget to set activeGame there
        SaveLoad.Load();
        GameRun.Init(SaveLoad.activeGame, SaveLoad.savedGames);
        GameRun.OpenMap();
    }

    private void OnLoadLevelEndScreen() {
        activeLoadingScreen = 3;
        missionEndScreen_child.gameObject.SetActive(true);
        missionEndScreen_child.GetComponentInChildren<TextAccess>().SetText(LevelRewardManager.m.AsText());
        Debug.Log("Todo: save the faction points into file.");
    }
}