using System;
using UnityEngine;
public abstract class GridData {
    public abstract void Null();
}
[System.Serializable]
public class Grid<T> where T: GridData{
    T[,] data = new T[0,0];
    private int width;
    private int length;

    public Grid(int width, int length) {
        this.width = width;
        this.length = length;
    }

    public void InitGrid(Vector2 posStart, Vector2 itemDimensions, Transform pref, Transform parent) {
        InitGrid(width, length, posStart, itemDimensions, pref, parent);
    }

    internal T GetItem(int x, int y) {
        if (x < width && y < length && y > -1 && x > -1)
            return data[x, y];
        return null;
    }

    public void InitGrid(int w, int l, Vector2 posStart, Vector2 itemSize, Transform itemPref, Transform parent) {
        if (w == 0 && l == 0)
            return;

        T[,] newGrid = new T[w, l];
        int wOld = data.GetLength(0), lOld = data.GetLength(1);
        for (int i = w; i < wOld; i++) {
            for (int j = 0; j < lOld; j++) {
                data[i, j].Null();
            }
        }

        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                if (i < wOld && j < lOld && data[i, j] != null) {
                    newGrid[i, j] = data[i, j];
                } else {
                    if (typeof(T).IsAssignableFrom(typeof(SceneTransformData))) {
                        newGrid[i, j] = new SceneTransformData(GameObject.Instantiate(itemPref)) as T;
                    }
                }
                if (typeof(T).IsAssignableFrom(typeof(SceneTransformData))) {
                    (newGrid[i, j] as SceneTransformData).transform.parent = parent;
                    (newGrid[i, j] as SceneTransformData).transform.position = posStart + new Vector2(itemSize.x * i, itemSize.y * j);
                }
            }
        }
        data = newGrid;
    }
}