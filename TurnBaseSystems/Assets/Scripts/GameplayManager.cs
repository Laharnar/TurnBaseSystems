using System.Collections;
using UnityEngine;
public class GameplayManager : MonoBehaviour {

    int activeFlagTurn = 0;


    Unit playerActiveUnit;

    private void Awake() {
        FlagManager.flags.Add(new PlayerFlag());
        FlagManager.flags.Add(new PlayerFlag());// enemy flag

        StartCoroutine(GameplayUpdate());
    }


    IEnumerator GameplayUpdate() {
        while (true) {
            for (int j = 0; j < FlagManager.flags.Count; j++) {
                bool allUnitsDone = true;
                do {
                    yield return StartCoroutine(FlagManager.flags[j].FlagUpdate());
                }
                while (!allUnitsDone);

                Debug.Log("Flag done - " + (j + 1));
            }
            yield return null;
        }
    }
}
