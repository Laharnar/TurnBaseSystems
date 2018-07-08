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

    private void Start() {
        m = this;
    }
    
    /*private void Update() {
        InitList(n);
    }*/

    public void InitList(int length) {
        for (int i = 0; i < instances.Count; i++) {
            if (instances[i] != null) {
                GameObject.Destroy(instances[i].gameObject);
            }
        }
        instances.Clear();

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

    public static void AssignInteractionToUI(Interaction interaction, int i) {
        m.instances[i].GetComponent<ButtonInteraction>().interaction = interaction;
        m.instances[i].GetChild(0).GetComponent<Text>().text += interaction.GetType();
    }

    internal static void LoadAbilitiesOnUI(Unit unit) {
        if (unit == null) {
            Debug.Log("No unit");
        }
        if (unit.abilities == null) {
            Debug.Log("No ability compoentn");
        }
        AttackData[] abilitis = unit.abilities.GetNormalAbilities();
        m.InitList(abilitis.Length);
        for (int i = 0; i < abilitis.Length; i++) {
            m.instances[i].GetComponent<ButtonInteraction>().interaction 
                = ScriptableObject.CreateInstance<TwoStepAttack>().Init(unit, i);
            m.instances[i].GetChild(0).GetComponent<Text>().text += " " + unit.abilities.GetNormalAbilities()[i].GetType().Name;

        }
    }
}
