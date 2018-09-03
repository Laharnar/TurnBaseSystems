using UnityEngine;
public static class CombatDebug {
    public static void Log(string msg) {
        Debug.Log(msg);
    }
    public static void Log(string msg, UnityEngine.Object target) {
        Debug.Log(msg, target);
    }
}
