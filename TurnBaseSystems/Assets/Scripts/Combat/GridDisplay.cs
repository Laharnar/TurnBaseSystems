using System;
using System.Collections.Generic;
using UnityEngine;
public class GridDisplay {
    public static List<GridLayer> layers = new List<GridLayer>();
    public static Vector3 center;
    public static List<GridDisplayItem> flattened = new List<GridDisplayItem>();

    public static List<Transform> instances = new List<Transform>();

    public static void RemakeGrid() {
        FlattenLayers();
        if (instances == null) instances = new List<Transform>();
        if (flattened == null) flattened = new List<GridDisplayItem>();
        // create if not enough
        for (int i = instances.Count; i < flattened.Count; i++) {
            Transform t = GridManager.NewGridPrefInstance(flattened[i].pos);
            int code = flattened[i].color;
            t.GetComponentInChildren<SpriteRenderer>().color =
                code == 0 ? GridManager.m.defaultColor :
                code == 1 ? Color.green :
                code == 2 ? Color.red :
                code == 3 ? Color.blue :
                code == 4 ? new Color(1, 0.6f, 0, 2) ://orange
                    Color.yellow;
            instances.Add(t);
        }
        // recolor and reposition
        for (int i = 0; i < instances.Count && i < flattened.Count; i++) {
            instances[i].transform.position = flattened[i].pos;
            int code = flattened[i].color;
            instances[i].GetComponentInChildren<SpriteRenderer>().color =
                code == 0 ? GridManager.m.defaultColor :
                code == 1 ? Color.green :
                code == 2 ? Color.red :
                code == 3 ? Color.blue :
                code == 4 ? new Color(1, 0.6f, 0, 2) ://orange
                    Color.yellow;
        }
        // remove if it's too much
        while (instances.Count > flattened.Count) {
            GameObject.Destroy(instances[instances.Count-1].gameObject);
            instances.RemoveAt(instances.Count-1);
        }
    }

    public static void SetUpGrid(Vector3 pos, int layer, int col, GridMask mask) {
        while (layers.Count <= layer && layer > -1) {
            layers.Add(new GridLayer());
        }
        Vector3[] positions = mask.GetPositions(pos);
        for (int i = 0; i < positions.Length; i++) {
            layers[layer].items.Add(new GridDisplayItem() { color = col, pos = positions[i] });
        }
    }

    public static void HideGrid(Vector3 pos, int layer, GridMask mask) {
        while (layers.Count <= layer && layer > -1) {
            layers.Add(new GridLayer());
        }
        Vector3[] positions = mask.GetPositions(pos);
        for (int i = 0; i < layers[layer].items.Count; i++) {
            if (mask.IsPosInMask(pos, layers[layer].items[i].pos)) {
                layers[layer].items.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// COSTLY, N^3
    /// </summary>
    /// <returns></returns>
    public static void FlattenLayers() {
        flattened.Clear();
        Vector2 lowerLeft=new Vector3(100000000,100000000,0), topRight=new Vector3(-10000000,-100000000,0);
        // start on front layer - top
        for (int i = layers.Count-1; i >= 0; i--) {
            for (int j = 0; j < layers[i].items.Count; j++) {
                Vector3 position = layers[i].items[j].pos;
                //find bounds
                if (position.x < lowerLeft.x) {
                    lowerLeft.x = position.x;
                }
                if (position.y < lowerLeft.y) {
                    lowerLeft.y = position.y;
                }
                if (position.x > topRight.x) {
                    topRight.x = position.x;
                }
                if (position.y > topRight.y) {
                    topRight.y = position.y;
                }
                // upper layer is taken 100%
                if (i < layers.Count - 1) {
                    bool matchingPos = false;
                    // skip positions on lower layers that match
                    for (int k = 0; k < flattened.Count; k++) {
                        if (flattened[k].pos == position) {
                            matchingPos = true;
                            break;
                        }
                    }
                    if (matchingPos) {
                        continue;
                    }
                }
                flattened.Add(new GridDisplayItem() { color = layers[i].items[j].color, pos = position });
            }
        }
        center = lowerLeft + (topRight - lowerLeft) / 2;
    }

    internal static void ClearAll() {
        for (int i = 0; i < instances.Count; i++) {
            GameObject.Destroy(instances[i].gameObject);
        }
        layers.Clear();
        instances.Clear();
        flattened.Clear();
        
    }

    internal static void MoveGrid(Vector3 opos, Vector3 npos, int layer, int col, GridMask mask) {
        HideGrid(opos, layer, mask);
        SetUpGrid(npos, layer, col, mask);
    }
}
public class GridLayer {
    public List<GridDisplayItem> items = new List<GridDisplayItem>();
}
public class GridDisplayItem {
    public Vector3 pos;
    public int color;
}
