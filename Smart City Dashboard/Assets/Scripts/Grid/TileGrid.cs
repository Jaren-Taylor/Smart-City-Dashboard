using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid
{

    /// <summary>
    /// Holds all the tiles present on the map
    /// </summary>
    private Tile[,] grid;

    public readonly int Width;
    public readonly int Height;

   
    public TileGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new Tile[width, height];
    }


    /// <summary>
    /// Gets access to the tile as this position
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Tile this[Vector2Int point]
    {
        get => this[point.x, point.y];
        set => this[point.x, point.y] = value;
    }

    /// <summary>
    /// Gets access to the tile as this position
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Checks if a point is in bounds given the current grid size
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool InBounds(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;


    /// <summary>
    /// From a point, get information about the neighbors
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public NeighborInfo GetNeighbors(Vector2Int point) => GetNeighbors(point.x, point.y);

    /// <summary>
    /// From a point, get information about the neighbors
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public NeighborInfo GetNeighbors(int x, int y)
    {
        NeighborInfo neighbors = new NeighborInfo(
            this[x-1, y], //Left Tile
            this[x+1, y], //Right Tile
            this[x, y+1], //Top Tile
            this[x, y-1]);//Bottom Tile
        return neighbors;
    }

    /// <summary>
    /// Formats the tilegrid for printing
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        string output = "Tilegrid Contents:\n";
        for (int x = 0; x < Width; x++) for (int y = 0; y < Height; y++) if (grid[x,y] != null) output += grid[x, y].ToString() + " : [" + x + ", " + y + "]\t";
        return output;
    }
}

/// <summary>
/// Struct to simply hold tiles. Represents the neighbors of a point
/// </summary>
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

