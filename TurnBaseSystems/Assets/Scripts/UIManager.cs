using System;
using UnityEngine;

public class UIManager :MonoBehaviour{

    public static UIManager m;
    public Transform pSelectAlly;
    public Transform playerUI;
    private void Awake() {
        m = this;
    }

    public static void PlayerSelectAllyUnitUi(bool v) {
        if (m.pSelectAlly)
        m.pSelectAlly.gameObject.SetActive(v);
    }

    internal static void PlayerStandardUi(bool v) {
        if (m.playerUI)
        m.playerUI.gameObject.SetActive(v);
    }
}
