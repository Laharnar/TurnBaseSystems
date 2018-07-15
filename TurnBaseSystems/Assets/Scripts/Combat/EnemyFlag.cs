using System;
using System.Collections;

public class EnemyFlag : FlagController {

    public override IEnumerator FlagUpdate() {
        NullifyUnits();
        Unit.activeUnit = null;

        // detect player units in range, and alert nearby allies
        foreach (var unit in units) {
            PlayerFlag pFlag = FlagManager.flags[0] as PlayerFlag;
            for (int i = 0; i < pFlag.units.Count; i++) {
                if (unit.detection.IsDetecting(unit, pFlag.units[i])) {
                    unit.detection.detectedSomeone = true;
                    GridItem[] units = unit.detection.GetGroup(unit);
                    for (int j = 0; j < units.Length; j++) {
                        units[j].filledBy.detection.detectedSomeone = true;
                    }
                }
            }
        }

        for (int i = 0; i < units.Count; i++) {
            Unit.activeUnit = units[i];
            UnityEngine.Debug.Log("running once...");
            yield return units[i].StartCoroutine(RunAi(units[i]));
            Unit.activeUnit = null;

            if (MissionManager.levelCompleted) {
                break;
            }
        }
        yield return null;
    }

    private IEnumerator RunAi(Unit unit) {
        // show enemy ui.
        yield return unit.StartCoroutine(unit.ai.Execute(unit));
    }
}
