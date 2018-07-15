using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameRun {

    public static GameRun current;

    /// <summary>
    /// Which map or mission save should be used.
    /// </summary>
    public static int currentSubSave = 0;

    bool playingInMission = false;
    List<SaveData> mapOrMissionSaves = new List<SaveData>();

    /// <summary>
    /// Sets up the game by loading data from the save, or makes a new save.
    /// </summary>
    /// <param name="activeGame"></param>
    /// <param name="savedGames"></param>
    internal static void Init(int activeGame, GameRun[] savedGames) {
        if (activeGame > savedGames.Length || activeGame < 0) {
            Debug.Log("Invalid range "+ activeGame);
            return;
        }
        if (savedGames[activeGame] == null) { // start a new game
            Debug.Log("New game created.");
            GameRun runInstance = new GameRun();
            runInstance.mapOrMissionSaves = new List<SaveData>();
            // ** startup setup **
            MapInfo startup = new MapInfo();
            startup.factions = FactionLibrary.FreshGameInit();
            // **
            runInstance.mapOrMissionSaves.Add(startup);
            SaveLoad.savedGames[activeGame] = runInstance;
            Debug.Log(SaveLoad.savedGames[activeGame]);
            GameRun.current = runInstance;
        }
        SaveLoad.Save();
    }

    public static void OpenMap() {
        // loads correct ui for factions, their characters, missions, completed missions
    }

    static void InitMission(GameRun game) {


    }
}
