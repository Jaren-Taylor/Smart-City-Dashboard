using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid 
{
    private TileType[,] grid;

    public readonly int Width;
    public readonly int Height;

    public TileGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new TileType[width, height];
    }

    public TileType this[Vector2Int point]
    {
        get => this[point.x, point.y];
        set => this[point.x, point.y] = value;
    }

    public TileType this[int x, int y]
    {
        get
        {
            if (!InBounds(x,y)) return TileType.OffGrid;
            else return grid[x, y];
        }
        set
        {
            if (!InBounds(x, y)) throw new IndexOutOfRangeException($"Cannot assign {value} to outside of grid boundaries.");
            if (value == TileType.OffGrid) throw new Exception("Cannot assign Off Grid type inside of grid boundaries");
            grid[x, y] = value;
        }
    }

    public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    public NeighborInfo GetNeighbors(int x, int y)
    {
        NeighborInfo neighbors = new NeighborInfo(
            this[x-1, y], //Left Tile
            this[x+1, y], //Right Tile
            this[x, y-1], //Top Tile
            this[x, y+1]);//Bottom Tile
        return neighbors;
    }
}

public struct NeighborInfo
{
    public readonly TileType left;
    public readonly TileType right;
    public readonly TileType top;
    public readonly TileType bottom;

    public NeighborInfo(TileType left, TileType right, TileType top, TileType bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}

public enum TileType
{
    Empty,
    Road,
    Structure,
    OffGrid
}
