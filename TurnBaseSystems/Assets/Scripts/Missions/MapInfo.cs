/// <summary>
/// mission end save(or map load save): 
/// before mission starts
/// selected team
/// faction points for all factions
/// skilltree
/// </summary>
[System.Serializable]
public class MapInfo: SaveData {
    Character[] activeTeam;
    FactionData[] factions;
    /// <summary>
    /// All possible player team characters in game.
    /// </summary>
    Character[] allPlayerControllableCharacters;
    // skill tree data, science tree, etc
}
