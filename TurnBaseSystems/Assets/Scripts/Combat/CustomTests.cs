using UnityEngine;
public class CustomTests: MonoBehaviour {
    static CustomTests m;
    int i=4;
    public bool showPassed = false;
    private void Awake() {
        m = this;
    }
    private void Update() {
        if (i == 0) {

            GridMask m = FlagManager.flags[1].units[0].abilities.move2.AttackMask;
            Vector3 r = new Vector3(-FlagManager.flags[1].units[0].abilities.move2.AttackMask.w / 2, 0, 0);

            Test("AiHelper.ClosestToTargetOverMask1", AiHelper.ClosestToTargetOverMask(new Vector3(), new Vector3(-100, 0, 0), m), r);
            Test("AiHelper.ClosestToTarget", AiHelper.ClosestToTarget(new Vector3(), new Vector3(-100, 0, 0), m), r);
            Print("Get mask positions", m.GetFreePositions(new Vector3()));
            Print("In mask", m.IsPosInMask(new Vector3(), new Vector3(0, 4,0)));

            float[] distsToSource = new float[] { 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2 };
            float[] distsToTarget = new float[] { 3, 4, 5, 4, 1, 2, 3, 4, 5, 4, 3, 2 };
            Test("AiHelper.IndexOfClosestToTarget", AiHelper.IndexOfClosestToTarget(distsToTarget, distsToSource), 4);
        }
            i--;
        
    }
    private static void Print(string context, bool data) {
        Debug.Log(context+" "+data);
    }
    private static void Print(string context, Vector3[] data) {
        string s = context;
        for (int i = 0; i < data.Length; i++) {
            s += " " + data[i];
        }
        Debug.Log(s);
    }

    private static void Test(string context, Vector3[] result, Vector3[] expected) {
        if (result.Length != expected.Length) {
            Debug.LogError("Test (" + context + ") failed: different length expected:" + expected.Length + " r:" + result.Length);
            return;
        }
        for (int i = 0; i < result.Length && i< expected.Length; i++) {
            if (result[i] != expected[i]) {
                Debug.LogError("Test (" + context + ") failed at ["+i+"]: expected:" + expected + " r:" + result);
            }
        }
        
    }
    private static void Test(string context, Vector3 result, Vector3 expected) {
        if (result != expected) {
            Debug.LogError("Test ("+context+") failed: expected:"+expected+" r:"+result);
        }
    }
    private static void Test(string context, int result, int expected) {
        if (result != expected) {
            Debug.LogError("Test (" + context + ") failed: expected:" + expected + " r:" + result);
        } else {
            if (m && m.showPassed) {
                Debug.Log("Test (" + context + ") passed.");
            }
        }
    }
}
