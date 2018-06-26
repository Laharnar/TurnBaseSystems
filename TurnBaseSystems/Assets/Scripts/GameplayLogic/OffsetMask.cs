using System;

public class OffsetMask {
    public int x;
    public int y;

    internal void ApplyOffset(ref int gridX, ref int gridY) {
        gridX += x;
        gridY += y;
    }
}

