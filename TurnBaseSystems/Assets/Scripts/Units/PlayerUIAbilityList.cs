using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start() {
        m = this;
    }
    
    /*private void Update() {
        InitList(n);
    }*/

    public void InitList(int length) {
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
        Debug.Log("Loading abilities");
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
                + (!abilitis[i].passive.used ? " (" +abilitis[i].actionCost+")" : "(Passive)");
            m.instances[i].GetComponent<Button>().interactable = allowInteraction&& unit.ActionsLeft >= abilitis[i].actionCost && !abilitis[i].passive.used;
            
        }
    }

    /// <summary>
    /// Marks the button with "overlay" object.
    /// </summary>
    /// <param name="btnObj">Set to null to not change position.</param>
    /// <param name="visible"></param>
    public void MarkButtonAsSelected(int btnId, bool visible) {
        Transform btnObj = btnId < instances.Count ? instances[btnId] : null;
        
        if (selectedButtonInstance == null) {
            selectedButtonInstance = Instantiate(selectedButtonPref, canvas.transform);
        }
        if (btnObj != null) {
            selectedButtonInstance.transform.position = btnObj.position+new Vector3(0,0,0.01f);
        }
        selectedButtonInstance.gameObject.SetActive(visible);
    }

}
