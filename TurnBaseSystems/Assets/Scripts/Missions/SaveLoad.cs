using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public static class SaveLoad {
    /// <summary>
    /// This should be set when starting a session.
    /// </summary>
    static int activeGame = 0;

    public static GameRun[] savedGames = new GameRun[3];

    public static void Save() {
        savedGames[activeGame] = GameRun.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
    }

    public static void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            SaveLoad.savedGames = (GameRun[])bf.Deserialize(file);
            file.Close();
        }
    }
}

[System.Serializable]
public class GameRun {
    public static GameRun current;

    bool playingInMission = false;
    List<MapInfo> map = new List<MapInfo>();

    List<MissionInfo> activeMissionSaves = new List<MissionInfo>();
}
/// <summary>
/// Current mission state info.
/// character positions, checkpoints, 
/// </summary>
[System.Serializable]
public class MissionInfo {
    public string missionName;
    public MissionCharacter[] allCharacters;
    public FactionCheckpoint[] allCheckpoints;
}

[System.Serializable]
public class MissionCharacter {
    public string codeName;
    int gridX, gridY;

}
/// <summary>
/// mission end save(or map load save): 
/// before mission starts
/// selected team
/// faction points for all factions
/// skilltree
/// </summary>
[System.Serializable]
public class MapInfo {
    Character[] activeTeam;
    FactionData[] factions;
    /// <summary>
    /// All possible player team characters in game.
    /// </summary>
    Character[] allPlayerControllableCharacters;
    // skill tree data, science tree, etc
}

[System.Serializable]
public class FactionData {
    public string name = "";
    public int pointsEarned = 0;
    public bool unlocked = false;
}

    /* multiple game runs
     * list of saves per run
     * optinally multiple mission and map save per run
     * 
     * mission autosaves - after turn ends
     * after mission ends
     * before mission starts
     * differnt content
     * 
     * rebuild game from lastest mission and map save
     * 
     * will we need mid-mission saves?
     * 
     * mission save:
     * all units positions, actions, hp, etc, etc
     * 
     * mission end save (or map load save): 
     * before mission starts
     * selected team
     * faction points for all factions
     * skilltree
     * */


[System.Serializable]
public class Character {

    public string name;
    public string faction;
    public int loyaltyEarned = 0;
    public bool unlocked = false;

    public Character() {
        this.name = "";
    }
}

public class CharacterLibrary:MonoBehaviour {
    public string[] characterCode;
    public Transform[] characterPrefs;

}