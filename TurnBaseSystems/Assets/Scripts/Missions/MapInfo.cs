/// <summary>
/// mission end save(or map load save): 
/// before mission starts
/// selected team
/// faction points for all factions
/// skilltree
/// </summary>
[System.Serializable]
public class MapInfo: SaveData {
    public Character[] activeTeam;
    public FactionData[] factions;
    /// <summary>
    /// All possible player team characters in game.
    /// </summary>
    public Character[] allPlayerControllableCharacters;
    // skill tree data, science tree, etc
}
