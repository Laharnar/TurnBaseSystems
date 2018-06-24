using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mask", menuName = "Grids/Mask", order = 1)]
[Serializable]
public class GridMask :UnityEngine.ScriptableObject {
    public int w, l;
    public BoolArr[] mask;

    public int LowerLeftFromX (int x) {
        return x - w / 2;
    }

    public int LowerLeftFromY(int y) {
        return y - l / 2;
    }

    internal GridMask Copy() {
        GridMask m = ScriptableObject.CreateInstance<GridMask>();
        m.w = w;
        m.l = l;
        m.mask = new BoolArr[w];
        for (int i = 0; i < w; i++) {
            m.mask[i] = new BoolArr();
            m.mask[i].col = new bool[l];
        }
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
        for (int i = 0; i < w; i++) {
            m.mask[i] = new BoolArr();
            m.mask[i].col = new bool[l];
        }
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                m.mask[i].col[j] = false;
            }
        }
        return m;
    }
    public float Range { get { return Mathf.Max(w, l); } }

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

    /*internal GridMask Init(List<GridItem> its) {
        int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
        for (int i = 0; i < its.Count; i++) {
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
        w = maxx - minx+1;
        l = maxy - miny+1;
        mask = new BoolArr[w];
        for (int i = 0; i < w; i++) {
            mask[i] = new BoolArr();
            mask[i].col = new bool[l];
        }
        for (int i = 0; i < its.Count; i++) {
            mask[its[i].gridX-minx].col[its[i].gridY-miny] = its[i];
        }
        return this;
    }*/
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