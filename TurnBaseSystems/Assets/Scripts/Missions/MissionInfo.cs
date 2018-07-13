/// <summary>
/// Current mission state info.
/// character positions, checkpoints, 
/// </summary>
[System.Serializable]
public class MissionInfo : SaveData {
    public string missionName;
    public MissionCharacter[] allCharacters;
    public FactionCheckpoint[] allCheckpoints;
}
[System.Serializable]
public abstract class SaveData {

}