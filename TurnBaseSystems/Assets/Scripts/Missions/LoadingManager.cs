using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class LoadingManager : MonoBehaviour {
    public static LoadingManager m;

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

    public static void OnLoadCharacterScreen() {
        m.activeLoadingScreen = 1;
        SceneManager.LoadScene("teamSelection");
    }


    public void OnLoadMission() {
        if (MissionManager.m && MissionManager.m.missionEndScreen_child)
            MissionManager.m.missionEndScreen_child.gameObject.SetActive(false);
        activeLoadingScreen = 2;
        SceneManager.LoadScene(GameRun.current.activeQuest.sceneName);
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

        GameRun.current = SaveLoad.savedGames[SaveLoad.activeGame];
        GameRun.OpenMap();
    }

    public static void ToMainMenu() {
        SceneManager.LoadScene("mapMissions");
    }
}