using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public static UIManager m;

    // whole player ui.
    public Transform pSelectAlly;
    public Transform playerUI;

    // slide in text at end of turns.
    public Text slide;
    public Animator slideAnim;
    internal string slideScreenContent;

    // additional select indicators.
    public Transform selectIndicatorPref;
    internal Vector3[] indicatorPositions;
    internal float indicatorTimeout = 1;

    public Transform descriptionPopupRoot;
    public Text descriptionPopupText;

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
        if (m.descriptionPopupRoot) {
            m.descriptionPopupRoot.gameObject.SetActive(v);
        }
        PlayerUIAbilityList.ClearInstanceList();
        if (unit) {
            PlayerUIAbilityList.LoadAbilitiesOnUI(unit, allowInteraction);
        }
    }

    internal static void ShowPlayerUI(bool v) {
        if (m.playerUI)
        m.playerUI.gameObject.SetActive(v);
    }

    public void ShowIndicators_evt() {
        ShowIndicators(indicatorPositions, indicatorTimeout);
    }

    public void ShowIndicators(Vector3[] pos, float visibilityTime) {
        for (int i = 0; i < pos.Length; i++) {
            Destroy(Instantiate(selectIndicatorPref, pos[i], new Quaternion()).gameObject,
                visibilityTime);
        }
    }

    internal static void ShowSlideMsg(string text, float time, string msg) {
        UIManager.m.slideScreenContent = text;
        CombatDisplayManager.Instance.Register(UIManager.m, "ShowSlideScreen", time, msg);
    }

    public static void ShowPopup(Vector3 rectPos, string text) {
        // disabled
        //(UIManager.m.descriptionPopupRoot as RectTransform).position = rectPos; 
        if (text == "")
            text = "No description.";
        m.descriptionPopupText.text = text;
    }
}
