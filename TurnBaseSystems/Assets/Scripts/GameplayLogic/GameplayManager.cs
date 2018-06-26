using System;
using System.Collections;
using UnityEngine;
public class GameplayManager : MonoBehaviour {

    int activeFlagTurn = 0;


    Unit playerActiveUnit;

    private void Awake() {
        FlagManager.flags.Add(new PlayerFlag());
        FlagManager.flags.Add(new EnemyFlag());

        StartCoroutine(GameplayUpdate());
    }


    IEnumerator GameplayUpdate() {
        yield return null;
        bool done = false;
        while (true) {
            for (int j = 0; j < FlagManager.flags.Count; j++) {
                activeFlagTurn = j;

                for (int i = 0; i < FlagManager.flags[j].units.Count; i++) {
                    FlagManager.flags[j].units[i].OnTurnStart();
                }
                yield return StartCoroutine(FlagManager.flags[j].FlagUpdate());

                for (int i = 0; i < FlagManager.flags[j].units.Count; i++) {
                    FlagManager.flags[j].units[i].OnTurnEnd();
                }
                Debug.Log("Flag done - " + (j + 1));
                FlagManager.flags[j].NullifyUnits();
                if (FlagManager.flags[0].units.Count == 0) {
                    yield return StartCoroutine(LoseGame());
                    done = true;
                    break;

                }
                // temp - win condition that enemy dies.
                if (FlagManager.flags[1].units.Count == 0) {
                    yield return StartCoroutine(WinGame());
                    done = true;
                    break;
                }
                yield return new WaitForSeconds(0.5f);

            }
            if (done) {
                break;
            }
            
            yield return null;
        }
    }

    private IEnumerator WinGame() {
        Debug.Log("WIN!");
        yield return null;
    }

    private IEnumerator LoseGame() {
        Debug.Log("Lose!");
        yield return null;
    }
}
