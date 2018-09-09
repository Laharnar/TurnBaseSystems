using UnityEngine;
public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    public CombatCam combatCam;
    [Header("Combat grid colors")]
    public Color globalColor = Color.black;
    public Color moveColor = Color.black;
    public Color attackColor = Color.black;
    public Color allySelectColor = Color.black;
    public Color enemySelectColor = Color.black;
    public Color aoeColor = Color.black;



    private void Awake() {
        Instance = this;

    }
}
