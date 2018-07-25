using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {
    public int a;
    public Text b;
    public int c; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (c==10) {
            a = a + 1;
            b.text = "" + a;
            c = 0;
        }
        c = c + 1;
        if (a>=60) {
            gameObject.SetActive(false);
        }
    }
}
