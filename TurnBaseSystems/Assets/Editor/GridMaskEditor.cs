using UnityEditor;

[CustomEditor(typeof(GridMask), true)]
public class GridMaskEditor : UnityEditor.Editor {

    bool lockMode = false;

    int x, y;

    public override void OnInspectorGUI() {
        GridMask source = target as GridMask;
        EditorGUI.BeginChangeCheck();
        if (source.mask == null) {
            source.mask = new BoolArr[0];
        }
        if (source.mask.Length != source.w || (source.mask.Length > 0 && source.mask[0].col.Length != source.l)) {
            source.mask = new BoolArr[source.w];
            for (int i = 0; i < source.w; i++) {
                source.mask[i] = new BoolArr() { col = new bool[source.w] };
            }
        }
        
        for (int i = 0; i < source.w; i++) {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < source.l; j++) {
                bool r = EditorGUILayout.Toggle(source.mask[i].col[j]);
                if (source.mask[i].col[j] !=r)
                    source.mask[i].col[j] = r;
            }
            EditorGUILayout.EndHorizontal();
        }

        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Changed");
        }
        EditorFix.SetObjectDirty(target);

    }
}
