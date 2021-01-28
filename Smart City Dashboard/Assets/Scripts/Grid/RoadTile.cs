using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Data structure to manage placed roads
/// </summary>
public class RoadTile : Tile
{
    public static readonly Dictionary<TileType, string> ModelLookup = new Dictionary<TileType, string>()
    {
        { TileType.Road0Way, "Prefabs/Roads/Road_0_Way"},
        { TileType.RoadEndcap, "Prefabs/Roads/Road_Endcap"},
        { TileType.Road2Way, "Prefabs/Roads/Road_2_Way"},
        { TileType.Road3Way, "Prefabs/Roads/Road_3_Way"},
        { TileType.Road4Way, "Prefabs/Roads/Road_4_Way"},
        { TileType.RoadCorner, "Prefabs/Roads/Road_Corner"}
    };

    public enum TileType { 
        Road0Way = 0,
        RoadEndcap = 1,
        Road2Way = 2,
        Road3Way = 3,
        Road4Way = 4,
        RoadCorner = 5
    }
    public TileType Type { get; private set; }



    protected override bool CalculateAndSetModelFromNeighbors(NeighborInfo neighbors)
    {
        int count = 0;
        bool left = false, right = false, top = false, bottom = false;

        if (neighbors.left is RoadTile) { count++; left = true; }
        if (neighbors.right is RoadTile) { count++; right = true; }
        if (neighbors.top is RoadTile) { count++; top = true; }
        if (neighbors.bottom is RoadTile) { count++; bottom = true; }

        // all this switch statement does it handle which way the tile should be rotated
        Facing rotation = Facing.Top;
        switch (count)
        {
            // 0 way
            case 0:
                Type = TileType.Road0Way;
                break;
            // end cap
            case 1:
                Type = TileType.RoadEndcap;
                if (right)
                {
                    rotation = Facing.Right;
                }
                else if (left)
                {
                    rotation = Facing.Left;
                }
                else if (bottom)
                {
                    rotation = Facing.Bottom;
                }
                break;
            // 2 way or corner
            case 2:
                if (top && bottom || left && right)
                {
                    Type = TileType.Road2Way;
                    if (right) rotation = Facing.Right;
                }
                else
                {
                    Type = TileType.RoadCorner;
                    if (right && top)
                    {
                        rotation = Facing.Right;
                    }
                    else if (left && bottom)
                    {
                        rotation = Facing.Left;
                    }
                    else if (right && bottom)
                    {
                        rotation = Facing.Bottom;
                    }
                }
                break;
            // 3 way
            case 3:
                Type = TileType.Road3Way;
                if (!left)
                {
                    rotation = Facing.Right;
                }
                else if (!right)
                {
                    rotation = Facing.Left;
                }
                else if (!top)
                {
                    rotation = Facing.Bottom;
                }
                break;
            // 4 way
            case 4:
                Type = TileType.Road4Way;
                break;
        }
        
        AttachModelToManaged(ModelLookup[Type], rotation); //Tells parent to construct the model in the orientation 
        return false;
    }
}
