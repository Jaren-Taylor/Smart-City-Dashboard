using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        Quaternion rotation = Quaternion.identity;
        switch (count)
        {
            // 0 way
            case 0:
                Type = TileType.Road0Way;
                break;
            // end cap
            case 1:
                Type = TileType.RoadEndcap;
                if (top)
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (right)
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                else if (left)
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                break;
            // 2 way or corner
            case 2:
                if (right && top || right && bottom || left && top || left && bottom)
                {
                    Type = TileType.RoadCorner;
                    if (right && top)
                    {
                        rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else if (right && bottom)
                    {
                        rotation = Quaternion.Euler(0, 90, 0);
                    }
                    else if (left && top)
                    {
                        rotation = Quaternion.Euler(0, -90, 0);
                    }
                }
                else
                {
                    Type = TileType.Road2Way;
                    if (right) rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            // 3 way
            case 3:
                Type = TileType.Road3Way;
                if (!bottom)
                {
                    rotation = Quaternion.Euler(0, 180, 0);
                }
                else if (!right)
                {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                else if (!left)
                {
                    rotation = Quaternion.Euler(0, 90, 0);
                }
                break;
            // 4 way
            case 4:
                Type = TileType.Road4Way;
                break;
        }
        
        AttachModelToManaged(ModelLookup[Type], rotation);
        return false;
    }
}
