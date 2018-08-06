using System;
using System.Collections.Generic;
using UnityEngine;

public enum GridDisplayLayer {
    GreenMovement,
    RedAttackArea,
    Aura,
    BlueSelectionArea,
    RedSelectionArea,
    SelfActivate,
    OrangeAOEAttack,
    OrangePierce,
    AIAction,
}
public class GridDisplay {
    public static List<GridLayer> layers = new List<GridLayer>();
    public static Vector3 center;
    public static List<GridDisplayItem> flattened = new List<GridDisplayItem>();

    public static List<Transform> instances = new List<Transform>();

    static Color GetColorCode(GridDisplayLayer layer) {
        switch (layer) {
            case GridDisplayLayer.GreenMovement:
                return Color.green;
            case GridDisplayLayer.RedAttackArea:
                return Color.red;
            case GridDisplayLayer.Aura:
                return Color.yellow;
            case GridDisplayLayer.BlueSelectionArea:
                return Color.blue;
            case GridDisplayLayer.RedSelectionArea:
                return Color.red;
            case GridDisplayLayer.SelfActivate:
                return Color.yellow;
            case GridDisplayLayer.OrangeAOEAttack:
                return new Color(1, 0.6f, 0, 2);//orange;
            case GridDisplayLayer.OrangePierce:
                return new Color(1, 0.6f, 0, 2);//orange;
            case GridDisplayLayer.AIAction:
                return Color.yellow;
            default:
                Debug.Log("Unhandled color, empty color.");
                return GridManager.m.defaultColor;
        }
    }

    public static void RemakeGrid() {
        FlattenLayers();
        if (instances == null) instances = new List<Transform>();
        if (flattened == null) flattened = new List<GridDisplayItem>();
        // create if not enough
        for (int i = instances.Count; i < flattened.Count; i++) {
            Transform t = GridManager.NewGridPrefInstance(flattened[i].pos);
            t.GetComponentInChildren<SpriteRenderer>().color = GetColorCode(flattened[i].color);
            instances.Add(t);
        }
        // recolor and reposition
        for (int i = 0; i < instances.Count && i < flattened.Count; i++) {
            instances[i].transform.position = flattened[i].pos;
            instances[i].GetComponentInChildren<SpriteRenderer>().color = GetColorCode(flattened[i].color);
        }
        // remove if it's too much
        while (instances.Count > flattened.Count) {
            GameObject.Destroy(instances[instances.Count-1].gameObject);
            instances.RemoveAt(instances.Count-1);
        }
    }

    public static void SetUpGrid(Vector3 pos, GridDisplayLayer layer, GridMask mask) {
        while (layers.Count <= (int)layer && (int)layer > -1) {
            layers.Add(new GridLayer());
        }
        Vector3[] positions = mask.GetPositions(pos);
        for (int i = 0; i < positions.Length; i++) {
            layers[(int)layer].items.Add(new GridDisplayItem() { color = layer, pos = positions[i] });
        }
    }

    public static void HideGrid(Vector3 pos, GridDisplayLayer layer, GridMask mask) {
        while (layers.Count <= (int)layer && (int)layer > -1) {
            layers.Add(new GridLayer());
        }
        Vector3[] positions = mask.GetPositions(pos);
        for (int i = 0; i < layers[(int)layer].items.Count; i++) {
            if (mask.IsPosInMask(pos, layers[(int)layer].items[i].pos)) {
                layers[(int)layer].items.RemoveAt(i);
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

    internal static void MoveGrid(Vector3 opos, Vector3 npos, GridDisplayLayer layer, GridMask mask) {
        HideGrid(opos, layer, mask);
        SetUpGrid(npos, layer, mask);
    }
    
}
public class GridLayer {
    public List<GridDisplayItem> items = new List<GridDisplayItem>();
}
public class GridDisplayItem {
    public Vector3 pos;
    public GridDisplayLayer color;
}
