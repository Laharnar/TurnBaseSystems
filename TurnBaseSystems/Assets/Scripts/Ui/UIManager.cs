using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager m;
    public Transform pSelectAlly;
    public Transform playerUI;

    public Text slide;
    public Animator slideAnim;
    internal string slideScreenContent;

    private void Awake() {
        m = this;
    }


    public void ShowSlideScreen() {
        Debug.Log("Showing slide screen");
        if (slide)
        slide.text = slideScreenContent;
        if (slideAnim)
        slideAnim.Play("Slide in");
    }

    public static void ShowAbilities(bool v, Unit unit, bool allowInteraction) {
        if (m.pSelectAlly)
            m.pSelectAlly.gameObject.SetActive(v);
        PlayerUIAbilityList.ClearInstanceList();
        if (unit)
            PlayerUIAbilityList.LoadAbilitiesOnUI(unit, allowInteraction);
    }

    internal static void ShowPlayerUI(bool v) {
        if (m.playerUI)
        m.playerUI.gameObject.SetActive(v);
    }
}
