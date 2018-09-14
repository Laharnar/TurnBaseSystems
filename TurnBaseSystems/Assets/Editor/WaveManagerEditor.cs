using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(WaveManager),true)]
public class WaveManagerEditor :Editor{
    int selectedForEditing = -1;
    public override void OnInspectorGUI() {
        WaveManager source = target as WaveManager;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        // extract swap class[1/2]
        if (!Application.isPlaying) {
            for (int i = 0; i < source.waves.Count; i++) {
                // title
                if (source.waves[i] == null) {
                    GUILayout.Label("Null value");
                } else {
                    GUILayout.Label(source.waves[i].description);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select")) {
                    selectedForEditing = i;
                }
                if (GUILayout.Button("+")) {
                    source.waves.Insert(i, null);
                }
                if (GUILayout.Button("-")) {
                    source.waves.RemoveAt(i);
                }
                if (selectedForEditing != -1 && i != selectedForEditing) {
                    if (GUILayout.Button("Swap " + i + " here")) {
                        Wave x = source.waves[selectedForEditing];
                        source.waves[selectedForEditing] = source.waves[i];
                        source.waves[i] = x;
                        selectedForEditing = -1;
                    }
                    if (GUILayout.Button("Swap unit_spawn" + i + " >")) {
                        Wave.SwapData(source.waves[selectedForEditing], source.waves[i], 1);
                        selectedForEditing = -1;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed character");
        }
        EditorFix.SetObjectDirty(target);
    }
}
