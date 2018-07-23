using System;

[System.Serializable]
public class QuestData {
    public int missionId;
    public string missionName = "*Classified*";
    public string sceneName;
    public string description;

    internal string GetDescription() {
        return "(id:" + missionId + ")\n" + missionName + " " + sceneName+"\n "+description;
    }
}
