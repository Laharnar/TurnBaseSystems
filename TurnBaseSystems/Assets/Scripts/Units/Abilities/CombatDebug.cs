using UnityEngine;
public static class CombatDebug {

    const int globalDebugLevel = 0;

    public static void Log(int debugLvl, string msg) {
        if (debugLvl >= globalDebugLevel)
            Log(msg);
    }

    public static void Log(string msg) {
        Debug.Log(msg);
    }
    public static void Log(string msg, UnityEngine.Object target) {
        Debug.Log(msg, target); 
    }
}
