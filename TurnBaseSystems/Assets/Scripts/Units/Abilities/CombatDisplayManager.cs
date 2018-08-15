using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DisplayCallInfo {
    public MonoBehaviour source;
    public string method;
    public float wait;
    public string context;

    DisplayCallInfo(MonoBehaviour source, string method, float wait) : this() {
        this.source = source;
        this.method = method;
        this.wait = wait;
    }

    public DisplayCallInfo(MonoBehaviour source, string method, float wait, string context) : this(source, method, wait) {
        this.context = context;
    }
}

public class CombatDisplayManager:MonoBehaviour {
    public static CombatDisplayManager Instance { get; private set; }

    public Queue<DisplayCallInfo> calls = new Queue<DisplayCallInfo>();

    private void Awake() {
        Instance = this;
        StartCoroutine(UpdateDisplay());
    }

    public void Register(MonoBehaviour obj, string method, float waitAfter, string context) {
        calls.Enqueue(new DisplayCallInfo(obj, method, waitAfter, context));
    }

    IEnumerator UpdateDisplay() {
        while (true) {
            if (calls.Count > 0) {
                DisplayCallInfo inf = calls.Dequeue();
                inf.source.Invoke(inf.method, 0);
                Debug.Log("INVOKE: "+inf.method + " wait: "+inf.wait + " ref:"+inf.source + " "+inf.context);
                yield return new WaitForSeconds(inf.wait);
            } else {
                yield return null;
            }
        }
    }

}
