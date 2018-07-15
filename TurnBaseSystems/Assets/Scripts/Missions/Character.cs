using System;
using UnityEngine;

[System.Serializable]
public class Character {

    public string name;
    public string faction;
    public int loyaltyEarned = 0;
    public bool unlocked = false;

    public Character() {
    }

    public Character(Unit unit) {
        name = unit.codename;
        faction =  FactionLibrary.factions[unit.factionId].name;
        loyaltyEarned = unit.loyalty;
        unlocked = unit.flag.allianceId == 0;
    }
}
