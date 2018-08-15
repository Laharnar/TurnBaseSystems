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

    public Transform selectIndicatorPref;
    internal Vector3[] indicatorPositions;
    internal float indicatorTimeout = 1;


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
}
