using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;


[DataContract]
public class TileGrid
{

    private const bool USE_OPTIMIZED_PATHFINDING = true;

    [DataMember(Name="Grid")]
    /// <summary>
    /// Holds all the tiles present on the map
    /// </summary>
    private Dictionary<Vector2Int, Tile> grid;

    private ReducedTileMap reducedTileMap;

    public Action<Vector2Int, Tile> positionChanged;

    [DataMember(Name="Width")]
    public readonly int Width;
    [DataMember(Name="Height")]
    public readonly int Height;

    private List<Vector2Int> cachedDestinations = null;
   
    public TileGrid(int width, int height)
    {
        Width = width;
        Height = height;
        grid = new Dictionary<Vector2Int, Tile>();//new Tile[width * height];
    }


    /// <summary>
    /// Gets access to the tile as this position
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Tile this[Vector2Int point]
    {
        get => SafeLookup(point);
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
            return SafeLookup(x, y);
        }
        set
        {
            if (!InBounds(x, y))
            {
                throw new IndexOutOfRangeException($"Cannot assign [{x}, {y}] to outside of grid boundaries.");
            }

            if (SafeLookup(x, y)?.ManagedExists() ?? false) {
                if (SafeLookup(x, y) is BuildingTile) cachedDestinations = null;
                SafeLookup(x, y).DeleteManaged();
            }
            if (value is BuildingTile) cachedDestinations = null;
            if (value == null) grid.Remove(new Vector2Int(x, y));
            else grid[new Vector2Int(x, y)] = value;
        }
    }

    private List<Vector2Int> GetLocations() => new List<Vector2Int>(grid.Keys);

    public bool Contains(Vector2Int position) => grid.ContainsKey(position);

    
    /// <summary>
    /// Checks to see if a 
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Vector2Int> GetBuildingLocations() => cachedDestinations ??= grid.Where(x => ((x.Value is BuildingTile) && (x.Value.IsPermanent))).Select(x => x.Key).ToList();
    public List<Vector2Int> GetRoadLocations() => grid.Where(x => ((x.Value is RoadTile) && (x.Value.IsPermanent))).Select(x => x.Key).ToList();

    private Tile SafeLookup(int x, int y) => grid.TryGetValue(new Vector2Int(x, y), out Tile output) ? output : null;
    private Tile SafeLookup(Vector2Int tilePos) => grid.TryGetValue(tilePos, out Tile output) ? output : null;

    internal void RefreshGrid()
    {
        var TileLocations = GetLocations();

        float total = TileLocations.Count;
        float current = 0f;

        foreach (var location in TileLocations)
        {
            grid[location].CreateManaged(location, GetNeighbors(location));
            grid[location].SpawnHeldSensors();
            current++;

            GridManager.Instance.LoadProgress = current / total;
        }

        GridManager.Instance.LoadProgress = 1f;

    }

    public void RoadAdded(Vector2Int position)
    {
        if (reducedTileMap is null) return;
        reducedTileMap.AddRoad(position);
    }

    public void RoadRemoved(Vector2Int position)
    {
        if (reducedTileMap is null) return;
        reducedTileMap.RemoveRoad(position);
    }

    public LinkedList<Vector2Int> GetListOfPositionsFromTo(Vector2Int fromTile, Vector2Int toTile)
    {
        if (USE_OPTIMIZED_PATHFINDING)
        {
            if(reducedTileMap is null) reducedTileMap = new ReducedTileMap(this);
            return reducedTileMap.GetListOfPositionsFromToReduced(fromTile, toTile);
        }
        else
        {
            return Pathfinding.GetListOfPositionsFromToReduced(fromTile, toTile);
        }
    }

    /// <summary>
    /// Checks if a point is in bounds given the current grid size
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns> bool </returns>
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
        foreach(KeyValuePair<Vector2Int, Tile> kvp in grid) output += $"[{kvp.Key}] : {kvp.Value}";

        //for (int x = 0; x < Width; x++) for (int y = 0; y < Height; y++) if (grid[xyToGrid(x, y)] != null) output += grid[xyToGrid(x, y)].ToString() + " : [" + x + ", " + y + "]< >";
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

    public int GetRoadCount()
    {
        int count = 0;
        if (left is RoadTile) count++;
        if (right is RoadTile) count++;
        if (top is RoadTile) count++;
        if (bottom is RoadTile) count++;

        return count;
    }
}

