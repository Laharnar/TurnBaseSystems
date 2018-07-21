using System;
using UnityEngine;
public static class CharacterLoader {
    public static void SaveActiveTeam(Character[] chars) {

    }

    public static void SaveUnlockedCharacters(Character[] chars) {

    }

    public static Transform[] LoadUnlockedCharacters() {
        Debug.Log("Get unlocked characters from currently loaded game. " +
            "Load from character library");
        return new Transform[0];
    }
    
    internal static Transform[] TempLoadTeam(int[] fastLoadTeam) {
        throw new NotImplementedException();
    }

    internal static Transform[] LoadActiveTeam() {
        return CharacterLibrary.CreateInstances(GameRun.current.currentMap.activeTeam);
    }
}

