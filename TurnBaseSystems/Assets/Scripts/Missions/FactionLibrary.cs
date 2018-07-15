public class FactionLibrary {

    public static FactionData[] factions;

    public static FactionData[] FreshGameInit() {
        factions = new FactionData[8]{
        new FactionData(){ name = "Players", pointsEarned =0, unlocked = true },
        new FactionData(){ name = "Monsters", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Empress", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Merchants", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Military", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Consumers", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Collectors", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Guilds", pointsEarned =0, unlocked = true }
        };
        return factions;
    }

}
