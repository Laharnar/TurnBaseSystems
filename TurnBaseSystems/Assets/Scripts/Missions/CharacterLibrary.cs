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

        // optional: load all possible characters and prefs from the file.
    }

    internal static Transform[] CreateInstances(int[] team) {
        if (!m)
            return new Transform[0];
        Transform[] instances = new Transform[team.Length];
        for (int i = 0; i < team.Length; i++) {
            Transform t = m.CreateInstance(team[i]);
            instances[i] = t;
        }
        return instances;
    }

    internal static Transform[] CreateInstances(Character[] team) {
        if (!m)
            return new Transform[0];
        Transform[] instances = new Transform[team.Length];
        for (int i = 0; i < team.Length; i++) {
            Transform t = m.CreateInstance(team[i]);
            instances[i] = t;
        }
        return instances;
    }

    public Transform CreateInstance(string code) {
        for (int i = 0; i < characterCode.Length; i++) {
            if (characterCode[i] == code) {
                return CreateInstance(i);
            }
        }
        Debug.Log("No character in library with name ." + code);
        return null;
    }
    private Transform CreateInstance(Character character) {
        return CreateInstance(character.name);
    }

    private Transform CreateInstance(int characteri) {
        if (characteri < characterCode.Length)
            return Instantiate(characterPrefs[characteri]);
        return null;
    }

    internal static Transform[] CreateEnemiesInstances(int[] team) {
        if (!m)
            return new Transform[0];
        Transform[] instances = new Transform[team.Length];
        for (int i = 0; i < team.Length; i++) {
            Transform t = m.CreateInstance(team[i]);
            instances[i] = t;
        }
        return instances;
    }

    internal static int GetId(string codename) {
        for (int i = 0; i < m.characterCode.Length; i++) {
            if (m.characterCode[i] == codename) {
                return i;
            }
        }
        return -1;
    }
}