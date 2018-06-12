using System;
using System.Collections;

public class EnemyFlag : FlagController {
    public override IEnumerator FlagUpdate() {
        NullifyUnits();
        for (int i = 0; i < units.Count; i++) {
            UnityEngine.Debug.Log("running once...");
            yield return units[i].StartCoroutine(RunAi(units[i]));
        }
        yield return null;
        turnDone = true;
    }

    private IEnumerator RunAi(Unit unit) {
        // show enemy ui.
        yield return unit.StartCoroutine(unit.ai.Execute(unit));
    }
}
