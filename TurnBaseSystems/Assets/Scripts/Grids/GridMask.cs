using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Mask", menuName = "Grids/Mask", order = 1)]
[Serializable]
public class GridMask :UnityEngine.ScriptableObject {
    public int w, l;
    public BoolArr[] mask;

    public bool Get(int i, int j) {
        return mask[i].col[j];
    }
}

[System.Serializable]
public class BoolArr {
    public bool[] col;
}