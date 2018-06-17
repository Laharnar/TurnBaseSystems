using System;
using System.Collections;

public class EnemyFlag : FlagController {

    public override IEnumerator FlagUpdate() {
        NullifyUnits();
        Unit.activeUnit = null;
        for (int i = 0; i < units.Count; i++) {
            Unit.activeUnit = units[i];
            UnityEngine.Debug.Log("running once...");
            yield return units[i].StartCoroutine(RunAi(units[i]));
            Unit.activeUnit = null;
        }
        yield return null;
        turnDone = true;
    }

    private IEnumerator RunAi(Unit unit) {
        // show enemy ui.
        yield return unit.StartCoroutine(unit.ai.Execute(unit));
    }
}
