using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad {
    /// <summary>
    /// This should be set when starting a session.
    /// </summary>
    public static int activeGame = 0;

    public static GameRun[] savedGames = new GameRun[3];

    public static void Save() {
        Debug.Log("Saved game "+ activeGame + " to "+Application.persistentDataPath + "/savedGames.gd");
        savedGames[activeGame] = GameRun.current;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, SaveLoad.savedGames);
        file.Close();
    }

    public static void Load() {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd")) {
            Debug.Log("Loaded game from "+ Application.persistentDataPath + "/savedGames.gd");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            SaveLoad.savedGames = (GameRun[])bf.Deserialize(file);
            file.Close();
        } else {
            Debug.Log("No files, skipping load.");
        }
    }
}
