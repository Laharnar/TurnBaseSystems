using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingManager : MonoBehaviour {
    public static LoadingManager m;
    internal MissionData activeMission;
    internal Character[] playerPickedTeam;

    int activeLoadingScreen = -1;


    private void Awake() {
        if (m != null)
            Destroy(gameObject);
        else {
            m = this;
            DontDestroyOnLoad(gameObject);
        }

        OnLoadMap();
    }

    private void OnLoadCharacterScreen() {
        activeLoadingScreen = 1;
        SceneManager.LoadScene("teamSelection");
    }


    private void OnLoadMission() {
        activeLoadingScreen = 2;
        SceneManager.LoadScene(activeMission.sceneName);
        StartCoroutine(LateMissionStartup());

    }

    private IEnumerator LateMissionStartup() {
        yield return new WaitForSeconds(2);

        MissionManager.m.Init(playerPickedTeam);
    }

    public void LoadNextScreen() {
        if (MissionManager.m && MissionManager.m.missionEndScreen_child)
            MissionManager.m.missionEndScreen_child.gameObject.SetActive(false);
        if (activeLoadingScreen == -1) {
            OnLoadMap();
        } else if (activeLoadingScreen == 0) {
            OnLoadCharacterScreen();
        } else if (activeLoadingScreen == 1) {
            OnLoadMission();
        } else if (activeLoadingScreen == 2) {
            activeLoadingScreen++;
        }
    }

    private void OnLoadMap() {
        activeLoadingScreen = 0;
        // TODO: Move this to main menu later, don't forget to set activeGame there
        SaveLoad.Load();
        GameRun.Init(SaveLoad.activeGame, SaveLoad.savedGames);
        GameRun.OpenMap();
    }

    public static void ToMainMenu() {
        SceneManager.LoadScene("mapMissions");
    }
}