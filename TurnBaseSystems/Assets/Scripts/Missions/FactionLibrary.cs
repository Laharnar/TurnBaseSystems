public class FactionLibrary {

    //public static FactionData[] curGameFactions;

    public static FactionData[] FreshGameInit() {
        return new FactionData[8]{
        new FactionData(){ name = "Players", pointsEarned =0, unlocked = true },
        new FactionData(){ name = "Monsters", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Empress", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Merchants", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Military", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Consumers", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Collectors", pointsEarned =0, unlocked = false },
        new FactionData(){ name = "Guilds", pointsEarned =0, unlocked = true }
        };
    }

}
