using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAccess : MonoBehaviour {

    public Text txt;

	// Use this for initialization
	void Awake() {
        if (txt == null)
            txt = GetComponent<Text>();
	}

    internal void SetText(string v) {
        txt.text = v;
    }
}
