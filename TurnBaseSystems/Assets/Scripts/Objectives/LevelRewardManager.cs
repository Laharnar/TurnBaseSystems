using System.Collections.Generic;
using UnityEngine;
public class LevelRewardManager:MonoBehaviour {
    public static LevelRewardManager m;

    List<Reward> collectedRewards = new List<Reward>();
    List<int> relatedFlag = new List<int>();

    private void Start() {
        m = this;
    }

    internal static void AddReward(Reward reward, Unit unit) {
        m.collectedRewards.Add(reward);
        m.relatedFlag.Add(unit.flag.allianceId);
    }

    public string AsText() {
        string s = "";
        for (int i = 0; i < collectedRewards.Count; i++) {
            s += collectedRewards[i] +" "+ relatedFlag[i]+"\n";
        }
        return s;
    }
}
