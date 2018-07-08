using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mask", menuName = "Grids/Mask", order = 1)]
[Serializable]
public class GridMask :UnityEngine.ScriptableObject {
    public int w, l;
    public BoolArr[] mask;
    public bool rotateable = false;

    public float Range { get { return Mathf.Max(w, l); } }

    public int LowerLeftFromX(int x) {
        return x - w / 2;
    }

    public int LowerLeftFromY(int y) {
        return y - l / 2;
    }

    public GridMask MirroredAxis() {
        GridMask m = EmptyCopy();
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                m.mask[i].col[j] = mask[j].col[i];
            }
        }
        return m;
    }
    /// <summary>
    /// Square masks only.
    /// </summary>
    /// <returns></returns>
    public GridMask MirroredHoriz() {
        GridMask m = EmptyCopy();
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                m.mask[i].col[j] = mask[w - i-1].col[j];
            }
        }
        return m;
    }/// <summary>
     /// Square masks only.
     /// </summary>
     /// <returns></returns>
    public GridMask MirroredVert() {
        GridMask m = EmptyCopy();
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                m.mask[i].col[j] = mask[i].col[l - j-1];
            }
        }
        return m;
    }
    internal GridMask Copy() {
        GridMask m = EmptyCopy();
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                m.mask[i].col[j] = mask[i].col[j];
            }
        }
        return m;
    }

    internal GridMask EmptyCopy() {
        GridMask m = ScriptableObject.CreateInstance<GridMask>();
        m.w = w;
        m.l = l;
        m.mask = new BoolArr[w];
        m.rotateable = rotateable;
        for (int i = 0; i < w; i++) {
            m.mask[i] = new BoolArr();
            m.mask[i].col = new bool[l];
        }
        return m;
    }

    public object CountActive {
        get {
            int count = 0;
            for (int i = 0; i < w; i++) {
                for (int j = 0; j < l; j++) {
                    if (mask[i].col[j]) count++;
                }
            }
            return count;
        }
    }

    public bool Get(int i, int j) {
        return mask[i].col[j];
    }

    /// <summary>
    /// Note: Mask must be marked as rotateable
    /// </summary>
    /// <param name="attackMask"></param>
    /// <param name="mouseDirection"></param>
    /// <returns></returns>
    internal static GridMask RotateMask(GridMask attackMask, int mouseDirection) {
        if (!attackMask.rotateable)
            return attackMask;
        switch (mouseDirection) {
            case 0: // r
                return attackMask;
            case 1: // up
                return attackMask.MirroredAxis();
            case 2: // l
                return attackMask.MirroredHoriz();
            case 3: // down
                return attackMask.MirroredAxis().MirroredVert();
            case -1:
                return attackMask;
            default:
                Debug.Log("unhandled value "+mouseDirection);
                return attackMask;
        }
    }
    
    /*
    internal static GridMask FullMask(GridItem[] items) {
        return CreateInstance<GridMask>().EmptyInit(items);

    }
    
    /// <summary>
    /// Note: doesn't work around edges.
    /// </summary>
    /// <param name="its"></param>
    /// <returns></returns>
    private GridMask EmptyInit(GridItem[] its) {
        int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
        for (int i = 0; i < its.Length; i++) {
            if (its[i].gridX < minx) {
                minx = its[i].gridX;
            }
            if (its[i].gridX > maxx) {
                maxx = its[i].gridX;
            }
            if (its[i].gridY < miny) {
                miny = its[i].gridY;
            }
            if (its[i].gridY > maxy) {
                maxy = its[i].gridY;
            }
        }
        w = maxx - minx + 1;
        l = maxy - miny + 1;
        Debug.Log(maxx + "-"+minx + "+1, "+maxy +"-"+miny+" +1");
        mask = new BoolArr[w];
        for (int i = 0; i < w; i++) {
            mask[i] = new BoolArr();
            mask[i].col = new bool[l];
        }
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                mask[i].col[j] = true;
            }
        }
        return this;
    }*/
}

[System.Serializable]
public class BoolArr {
    public bool[] col;
}