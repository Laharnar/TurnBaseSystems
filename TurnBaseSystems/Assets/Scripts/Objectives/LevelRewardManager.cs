using System.Collections.Generic;
using UnityEngine;
public class LevelRewardManager:MonoBehaviour {
    public static LevelRewardManager m;
    List<Reward> collectedRewards = new List<Reward>();
    List<int> relatedFlag = new List<int>();

    internal static void AddReward(Reward reward, Unit unit) {
        m.collectedRewards.Add(reward);
        m.relatedFlag.Add(unit.flag.allianceId);
    }
}