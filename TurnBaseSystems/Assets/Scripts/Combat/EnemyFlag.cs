using System;
using System.Collections;
using System.Collections.Generic;

public class EnemyFlag : FlagController {

    public override IEnumerator FlagUpdate(FlagManager flag) {
        List<Unit> units = flag.info.units;
        // detect player units in range, and alert nearby allies
        //HandleDetectionAndAlert(flag, units);

        for (int i = 0; i < units.Count; i++) {
            yield return units[i].StartCoroutine(units[i].ai.Execute(units[i]));

            if (MissionManager.levelCompleted) {
                break;
            }
        }
        yield return null;
    }

    private static void HandleDetectionAndAlert(FlagManager flag, List<Unit> units) {
        foreach (var unit in units) {
            for (int i = 0; i < units.Count; i++) {
                if (unit.detection.IsDetecting(unit, units[i])) {
                    unit.detection.detectedSomeone = true;
                    Unit[] detected = unit.detection.GetGroup(unit);
                    for (int j = 0; j < detected.Length; j++) {
                        detected[j].detection.detectedSomeone = true;
                    }
                }
            }
        }
    }

}
