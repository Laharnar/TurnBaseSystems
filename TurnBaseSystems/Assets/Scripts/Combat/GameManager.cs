using UnityEngine;
public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public CombatCam combatCam;

    private void Awake() {
        Instance = this;
    }
}
