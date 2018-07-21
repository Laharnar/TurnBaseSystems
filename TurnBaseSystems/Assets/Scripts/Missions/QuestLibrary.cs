using System;

public sealed class QuestLibrary {
    internal static QuestData[] LoadMissions() {
        MapInfo s = GameRun.current.currentMap;
        return s.quests;
    }
    internal static QuestData GetQuestDataFromSavedMission(MissionSaveData activeQuest) {
        MapInfo s = (GameRun.current.mapOrMissionSaves[GameRun.currentSubSave] as MapInfo);
        for (int i = 0; i < s.quests.Length; i++) {
            if (s.quests[i].missionName == activeQuest.missionName) {
                return s.quests[i];
            }
        }
        return null;
    }
}