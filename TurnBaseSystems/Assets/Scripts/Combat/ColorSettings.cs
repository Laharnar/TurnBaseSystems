using System;
using UnityEngine;
[System.Serializable]
public class ColorSettings {
    public Color alliesColor, enemyColor;

    public Color GetColor(Unit unit) {
        return GetColor(unit.flag.allianceId);
    }
    public Color GetColor(int id) {
        if (id == 1) {
            return enemyColor;
        }
        return alliesColor;
    }

    internal Color GetColor(object activeFlag) {
        throw new NotImplementedException();
    }
}
