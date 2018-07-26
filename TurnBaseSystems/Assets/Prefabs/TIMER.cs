using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TIMER : MonoBehaviour {
    int i = 0;
    int step = 0;
    public Text a;
    // Use this for initialization
    void Start () {
       
    }
	
	// Update is called once per frame
	void Update () {
        
            Debug.Log("checks");
        if (step == 10) {
            a.text = ""+i;
            i++;
            step = 0;
        }
        step++;

    }
}
