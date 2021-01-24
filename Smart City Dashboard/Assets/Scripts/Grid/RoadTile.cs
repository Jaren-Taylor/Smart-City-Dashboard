using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RoadTile : Structure
{
    public enum TileType { 
        Road0Way = 0,
        RoadEndCap = 1,
        Road2Way = 2,
        Road3Way = 3,
        Road4Way = 4,
        RoadCorner = 5
    }
    public TileType Type;
}
