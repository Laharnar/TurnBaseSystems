﻿using System;
using UnityEngine;
/// <summary>
/// Can create character instances depending on character code.
/// </summary>
public class CharacterLibrary:MonoBehaviour {
    public static CharacterLibrary m;
    public string[] characterCode;
    public Transform[] characterPrefs;
    private void Awake() {
        if (m) {
            Debug.Log("Removed double character library.");
            Destroy(this);
        }
        m = this;
    }

    internal static Transform[] CreateInstances(Character[] team) {
        Transform[] instances = new Transform[team.Length];
        for (int i = 0; i < team.Length; i++) {
            Transform t = m.GetInstance(team[i]);
            instances[i] = t;
        }
        return instances;
    }
    
    private Transform GetInstance(Character character) {
        for (int i = 0; i < characterCode.Length; i++) {
            if (characterCode[i] == character.name) {
                return Instantiate(characterPrefs[i]);
            }
        }
        return null;
    }

}