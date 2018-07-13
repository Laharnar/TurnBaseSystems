[System.Serializable]
public class Character {

    public string name;
    public string faction;
    public int loyaltyEarned = 0;
    public bool unlocked = false;

    public Character() {
        this.name = "";
    }
}
