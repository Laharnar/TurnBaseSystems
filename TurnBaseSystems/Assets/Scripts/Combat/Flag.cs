using System;


public class Flag {
    public FlagInfo info = new FlagInfo();
    public FlagBehaviour controller;
    public int id;

    public Flag(FlagBehaviour enemyFlag, int id) {
        info = new FlagInfo();
        this.controller = enemyFlag;
        this.id = id;
    }

    public void NullifyUnits() {
        info.NullifyUnits();
    }
}