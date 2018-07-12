using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Reward {
    public int faction1Points = 0;
    public int faction2Points = 0;
    public int faction3Points = 0;
    public int faction4Points = 0;
    public int factionEnemyPoints = 0;
}
public enum MaskFilter {
    Anyone,
    PlayerTeam,
    AlliedFactions,
    EnemyFactions,
    Civialians,
    SpecificFaction1,
}
public class FactionCheckpoint : MonoBehaviour {

    public static List<FactionCheckpoint> checkpointsInLevel = new List<FactionCheckpoint>();

    public GridMask rangeCheck;
    public MaskFilter checkpointTrigger;

    public Reward reward;

    public bool isMissionGoal = false;
    [SerializeField] Unit alreadyUsed = null;
    [SerializeField] GridItem slot;

	// Use this for initialization
	void Start () {
         slot = GridManager.SnapToGrid(transform.position);
        transform.position = slot.transform.position;
        checkpointsInLevel.Add(this);
    }

    public void CheckpointCheck(Unit other) {
        if (ConditionsMet(other)) {
            ActivateCheckpoint(other);
        }
    }

    public bool ConditionsMet(Unit other) {
        return !alreadyUsed && GridLookup.IsSlotInMask(slot, other.curSlot, rangeCheck)
            && FactionsMatch(other, checkpointTrigger);
    }

    // Update is called once per frame
    void ActivateCheckpoint (Unit unit) {
        alreadyUsed = unit;
        GetComponentInChildren<SpriteRenderer>().transform.localScale *= 3;
        GameplayManager.OnEnterCheckPoint(this, unit);
	}

    public static bool FactionsMatch(Unit other, MaskFilter checkpointTrigger) {
        switch (checkpointTrigger) {
            case MaskFilter.Anyone:
                return true;
            case MaskFilter.PlayerTeam:
                return other.flag.allianceId == 0;
            case MaskFilter.AlliedFactions:
                return other.flag.allianceId == 0;
            case MaskFilter.EnemyFactions:
                return other.flag.allianceId != 0;
            case MaskFilter.Civialians:
                return other.flag.allianceId == 100;
            case MaskFilter.SpecificFaction1:
                Debug.Log("Specific factions aren't implemented yet.");
                break;
            default:
                Debug.Log("Unhandled value " + checkpointTrigger);
                break;
        }
        return false;
    }

}
