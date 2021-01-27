using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid 
{
    private Tile[,] grid;

    public readonly int Width;
    public readonly int Height;

    public TileGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new Tile[width, height];
    }

    public Tile this[Vector2Int point]
    {
        get => this[point.x, point.y];
        set => this[point.x, point.y] = value;
    }

    public Tile this[int x, int y]
    {
        get
        {
            if (!InBounds(x,y)) return null;
            else return grid[x, y];
        }
        set
        {
            if (!InBounds(x, y))
            {
                throw new IndexOutOfRangeException($"Cannot assign [{x}, {y}] to outside of grid boundaries.");
            }
            if (grid[x, y]?.ManagedExists() ?? false) grid[x, y].DeleteManaged();
            grid[x, y] = value;
        }
    }

    public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public NeighborInfo GetNeighbors(Vector2Int point) => GetNeighbors(point.x, point.y);

    public NeighborInfo GetNeighbors(int x, int y)
    {
        NeighborInfo neighbors = new NeighborInfo(
            this[x-1, y], //Left Tile
            this[x+1, y], //Right Tile
            this[x, y-1], //Top Tile
            this[x, y+1]);//Bottom Tile
        return neighbors;
    }

    public override string ToString()
    {
        string output = "Tilegrid Contents:\n";
        for (int x = 0; x < Width; x++) for (int y = 0; y < Height; y++) if (grid[x,y] != null) output += grid[x, y].ToString() + " : [" + x + ", " + y + "]\t";
        return output;
    }
}

public struct NeighborInfo
{
    public readonly Tile left;
    public readonly Tile right;
    public readonly Tile top;
    public readonly Tile bottom;

    public NeighborInfo(Tile left, Tile right, Tile top, Tile bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}

