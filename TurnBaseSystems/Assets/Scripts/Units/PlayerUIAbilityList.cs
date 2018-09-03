using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays buttons for player abilities.
/// </summary>
public class PlayerUIAbilityList : MonoBehaviour {

    public static PlayerUIAbilityList m;

    public Canvas canvas;
    public Transform buttonPref;
    public float offset;
    public float height = 40;
    public int n = 1;

    List<Transform> instances = new List<Transform>();

    public Transform selectedButtonPref;
    Transform selectedButtonInstance;
    Coroutine slideBetweenChoices;

    public bool showAbilityDescription  =false;

    private void Start() {
        m = this;
    }
    
    /*private void Update() {
        InitList(n);
    }*/

    public void InitList(int length) { 
        if (length == instances.Count) {
            return;
        }
        ClearInstanceList();

        Vector2 screenLeft = new Vector2(-Camera.main.pixelWidth/2, 0);
        Vector2 screenBot = screenLeft + new Vector2(height, (length - 1) * offset / 2);

        for (int i = 0; i < length; i++) {
            Transform t = Instantiate(buttonPref, new Vector3(), new Quaternion());
            t.SetParent(canvas.transform, false);
            t.name += i;
            (t as RectTransform).anchoredPosition = screenBot - new Vector2(0, i * offset);
            instances.Add(t);
        }
    }

    public static void ClearInstanceList() {
        for (int i = 0; i < m.instances.Count; i++) {
            if (m.instances[i] != null) {
                GameObject.Destroy(m.instances[i].gameObject);
            }
        }
        m.instances.Clear();
    }

    internal static void LoadAbilitiesOnUI(Unit unit, bool allowInteraction) {
        if (unit == null) {
            Debug.Log("No unit");
        }
        if (unit.abilities == null) {
            Debug.Log("No ability compoentn");
        }
        AttackData2[] abilitis = unit.abilities.GetNormalAbilities() as AttackData2[];
        m.InitList(abilitis.Length);
        for (int i = 0; i < abilitis.Length; i++) {
            if (abilitis[i].active == false) {
                m.instances[i].gameObject.SetActive(false);
                continue;
            }
            m.instances[i].GetComponent<ButtonInteraction>().interaction 
                = ScriptableObject.CreateInstance<TwoStepAttack>().Init(unit, i);
            m.instances[i].GetChild(0).GetComponent<Text>().text = abilitis[i].o_attackName
                + (!abilitis[i].passive.used ? " (AP:" +abilitis[i].actionCost+")" : "(Passive)");
            m.instances[i].GetComponent<Button>().interactable = allowInteraction && unit.PassGameRules(abilitis[i]) && !abilitis[i].passive.used;
            
        }
    }
    /// <summary>
    /// Marks the button with "overlay" object.
    /// </summary>
    /// <param name="btnObj">Set to null to not change position.</param>
    /// <param name="visible"></param>
    public void MarkButtonAsSelected(int btnId, bool visible) {
        Transform btnObj = btnId < instances.Count ? instances[btnId] : null;
        // create overlay
        if (selectedButtonInstance == null) {
            selectedButtonInstance = Instantiate(selectedButtonPref, canvas.transform);
            selectedButtonInstance.transform.position = btnObj.position+new Vector3(0,0,0.01f);
            selectedButtonInstance.SetSiblingIndex(0);
        }
        if (btnObj != null) {
            if (slideBetweenChoices!= null) StopCoroutine(slideBetweenChoices);
            slideBetweenChoices = StartCoroutine(LerpTo(selectedButtonInstance.transform, btnObj.position + new Vector3(0, 0, 0.01f), 0.25f));
        }
        selectedButtonInstance.gameObject.SetActive(visible);

        if (btnObj != null && AbilityInfo.ActiveAbility!= null) {// ability is null when enemy is selected, and btn is null in enemy turn
            UIManager.ShowPopup(btnObj.position, AbilityInfo.ActiveAbility.detailedDescription);
        }
    }


    private IEnumerator LerpTo(Transform transform, Vector3 vector3, float inTime) {
        float t = 0;
        for (int i = 0; i < inTime/Time.deltaTime; i++) {
            transform.position=Vector3.Lerp(transform.position, vector3, i/(inTime / Time.deltaTime));
            yield return null;
        }
        transform.position = Vector3.Lerp(transform.position, vector3, 1);
    }
}
