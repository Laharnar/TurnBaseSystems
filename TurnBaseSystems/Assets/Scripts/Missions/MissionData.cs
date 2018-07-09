using System;

[System.Serializable]
public class MissionData {
    public int missionId;
    public string missionName = "*Classified*";
    public string sceneName;

    internal string GetDescription() {
        return "(id:" + missionId + ")\n" + missionName + " " + sceneName;
    }
}
