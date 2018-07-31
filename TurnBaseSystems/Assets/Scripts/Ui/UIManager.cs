using System;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager m;
    public Transform pSelectAlly;
    public Transform playerUI;
    private void Awake() {
        m = this;
    }

    public static void ShowAbilities(bool v, Unit unit) {
        if (m.pSelectAlly)
            m.pSelectAlly.gameObject.SetActive(v);
        PlayerUIAbilityList.ClearInstanceList();
        if (unit)
            PlayerUIAbilityList.LoadAbilitiesOnUI(unit);
    }

    internal static void ShowPlayerUI(bool v) {
        if (m.playerUI)
        m.playerUI.gameObject.SetActive(v);
    }
}
