using System;
using UnityEngine;

[System.Serializable]
public class Grid{
    public GridItem[,] data { get; private set; }
    private GridMask mask;
    int width { get { return mask.w; } }
    int length { get { return mask.l; } }

    public Grid(GridMask mask) {
        this.mask = mask;
    }

    public Grid(GridMask mask, Transform rootLoader) {
        this.mask = mask;
        int width = mask.w, length = mask.l;
        data = new GridItem[width, length];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                int id = i*length+j;
                if (id < rootLoader.childCount) {
                    data[i, j] = new GridItem(){instance=rootLoader.GetChild(id)};
                    //data[i, j].InitGrid(i, j);
                }
            }
        }
    }

    internal void ShowColor(int v) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                if (data[i, j] == null)
                    continue;
                data[i, j].RecolorSlot(v);
            }
        }
    }

    /// <summary>
    /// Pass null to destroy whole grid.
    /// </summary>
    /// <param name="mask"></param>
    internal void HalfRemove(GridMask mask) {
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                if ((mask && mask.Get(i, j) == false && data[i, j] == null) ||data[i,j]==null)
                    continue;
                
                GameObject.Destroy(data[i, j].instance.gameObject);
                data[i, j] = null;
            }
        }
    }

    public Grid InitGridCenter(Vector2 center, GridMask mask) {
        data = CreateGrid(width, length, center - new Vector2(GridManager.m.itemDimensions.x * (width-1) / 2, GridManager.m.itemDimensions.y * (length-1) / 2), GridManager.m.itemDimensions, GridManager.m.pref, GridManager.m.gridParent, mask);
        return this;
    }

    internal GridItem GetItem(int i, int j) {
        if (i < width && j < length && j > -1 && i > -1)
            return data[i, j];
        Debug.LogError("Grid/GetItem - Out of range exception: "+i +" "+ j+" " + width+" "+length);
        return null;
    }
    /// <summary>
    /// Assumes existance of grid item with this position.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="gridCenter"></param>
    /// <returns></returns>
    internal GridItem GetItemByXY(Vector3 pos) {
        pos = GridManager.SnapPoint(pos);
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < length; j++) {
                if (data[i, j].worldPosition == pos)
                    return data[i, j];
            }
        }
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

    public static GridItem[,] CreateGrid(int w, int l, Vector2 posStart, Vector2 itemSize, Transform itemPref, Transform parent, GridMask mask) {
        if (w == 0 && l == 0)
            return null;

        GridItem[,] newGrid = new GridItem[w, l];
        int skip = 0;
        for (int i = 0; i < w; i++) {
            for (int j = 0; j < l; j++) {
                if (mask!=null && !mask.Get(i, j)) {
                    skip ++;
                    continue;
                }

                newGrid[i, j] = GridManager.NewGridInstance(posStart + new Vector2(itemSize.x * i, itemSize.y * j));
                    //new GridItem() { instance = GameObject.Instantiate(itemPref) };//.GetComponent<GridItem>();
                /*newGrid[i, j].instance.parent = parent;
                newGrid[i, j].instance.name = "("+i+":"+j+")"+ newGrid[i, j].instance.name;
                newGrid[i, j].position = posStart + new Vector2(itemSize.x * i, itemSize.y * j);
                newGrid[i, j].gridX = i;
                newGrid[i, j].gridY = j;*/
            }
        }
        Debug.Log("New grid. "+w + " "+l+ " skipped:"+skip+" "+((float)skip/((float)w*l)));
        return newGrid;
    }
}