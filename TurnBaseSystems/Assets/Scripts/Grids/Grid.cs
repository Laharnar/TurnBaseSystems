using System;
using UnityEngine;

[System.Serializable]
public class Grid<T> where T: GridItem{
    public T[,] data { get; private set; }
    private int width;
    private int length;

    public Grid(int width, int length) {
        this.width = width;
        this.length = length;
    }

    public Grid(int width, int length, Transform rootLoader):this(width, length) {
        data = new T[width, length];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                int id = i*length+j;
                data[i, j] = rootLoader.GetChild(id).GetComponent<T>();
                data[i, j].InitGrid(i, j);
            }
        }
    }

    public void InitGrid(Vector2 posStart, Vector2 itemDimensions, Transform pref, Transform parent) {
        InitGrid(width, length, posStart, itemDimensions, pref, parent);
    }

    internal T GetItem(int x, int y) {
        if (x < width && y < length && y > -1 && x > -1)
            return data[x, y];
        Debug.LogError("Grid/GetItem - Out of range exception: "+x +" "+ y+" " + width+" "+length);
        return null;
    }


    public GridItem[] AsArray() {
        GridItem[] arr = new GridItem[width * length];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                arr[j + i * length] = data[i,j];
            }
        }
        return arr;
    }

    public void InitGrid(int w, int l, Vector2 posStart, Vector2 itemSize, Transform itemPref, Transform parent) {
        if (w == 0 && l == 0)
            return;

        T[,] newGrid = new T[w, l];
        int wOld = data.GetLength(0), lOld = data.GetLength(1);
        for (int i = w; i < wOld; i++) {
            for (int j = 0; j < lOld; j++) {
                data[i, j].Null();
                data[i, j] = null;
            }
        }

        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                /*if (i < wOld && j < lOld && data[i, j] != null) {
                    newGrid[i, j] = data[i, j];
                } else */{
                    newGrid[i, j] = GameObject.Instantiate(itemPref).GetComponent<GridItem>() as T;
                }
                newGrid[i, j].transform.parent = parent;
                newGrid[i, j].transform.name = "("+i+":"+j+")"+ newGrid[i, j].name;
                newGrid[i, j].transform.position = posStart + new Vector2(itemSize.x * i, itemSize.y * j);
                newGrid[i, j].gridX = i;
                newGrid[i, j].gridY = j;
            }
        }
        data = newGrid;
    }
}